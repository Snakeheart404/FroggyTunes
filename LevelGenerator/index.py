import logging
from flask import Flask, request, jsonify, send_file
import os
import tempfile
import zipfile
from pathlib import Path
import librosa
import numpy as np
import av
from sklearn.cluster import KMeans
from tinytag import TinyTag
from waitress import serve

app = Flask(__name__)

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

def export_to_wav(input_path, sample_rate: int = 41000):
    output_path = os.path.splitext(input_path)[0] + '.wav'
    with av.open(input_path) as input_file:
        input_stream = input_file.streams.audio[0]
        with av.open(output_path, 'w', 'wav') as output_file:
            output_stream = output_file.add_stream('pcm_s16le', rate=sample_rate)
            for frame in input_file.decode(input_stream):
                for packet in output_stream.encode(frame):
                    output_file.mux(packet)
    return output_path

def delete_close(onsets_time, threshold):
    i = 0
    while i < (len(onsets_time) - 1):
        if abs(onsets_time[i] - onsets_time[i + 1]) < threshold:
            onsets_time = np.delete(onsets_time, i + 1)
        else:
            i += 1
    return onsets_time

def mfcc_processing(audio_file, sample_rate, onsets_time):
    hop_length = 512
    mfcc = librosa.feature.mfcc(y=audio_file, sr=sample_rate, hop_length=hop_length)
    timings = librosa.frames_to_time(np.arange(mfcc.shape[1]), sr=sample_rate, hop_length=hop_length)
    channels_mfcc = np.zeros((len(onsets_time), mfcc.shape[0]))
    for i, onset in enumerate(onsets_time):
        index = np.argmin(np.abs(timings - onset))
        channels_mfcc[i] = mfcc[:, index]
    return channels_mfcc

def assign_channels(channels_mfcc, clusters_number: int = 4):
    samples_number = channels_mfcc.shape[0]
    clusters_number = min(clusters_number, samples_number)
    k_means_clustering = KMeans(n_clusters=clusters_number)
    k_means_clustering.fit(channels_mfcc)
    return k_means_clustering.labels_

def process_data(file_path):
    name = Path(file_path).stem
    extension = Path(file_path).suffix
    path_without_extension = os.path.splitext(file_path)[0]
    tag = TinyTag.get(file_path)
    artist = tag.artist
    if extension != ".wav":
        file_path = export_to_wav(file_path)
    audio_file, sample_rate = librosa.load(file_path)
    onsets = librosa.onset.onset_detect(y=audio_file, sr=sample_rate, wait=5, pre_avg=1, post_avg=1, pre_max=1, post_max=1)
    onsets_time = librosa.frames_to_time(onsets, sr=sample_rate)
    onsets_time = delete_close(onsets_time, 0.25)
    channels_mfcc = mfcc_processing(audio_file, sample_rate, onsets_time)
    channels = assign_channels(channels_mfcc)
    txt_output = write_file(onsets_time, channels, path_without_extension, name, artist)
    return file_path, txt_output

def write_file(notes, channels, source, name, artist):
    filename = source + ".txt"
    with open(filename, 'w', encoding='utf-8') as file:
        file.write("[INFO]\n")
        file.write(f"Name: {name}\n\n")
        file.write(f"Artist: {artist} \n\n")
        file.write(f"Source: {source}.wav\n\n")
        file.write("[NOTES]\n")
        for note, channel in zip(notes, channels):
            file.write(f"{note} {channel+1}\n")
    return filename

@app.route("/", methods=["POST"])
def process_request():
    if 'file' not in request.files:
        return jsonify({"status": "error", "message": "No file part"}), 400
    file = request.files['file']
    if file.filename == '':
        return jsonify({"status": "error", "message": "No selected file"}), 400
    try:
        with tempfile.NamedTemporaryFile(delete=False, suffix=Path(file.filename).suffix) as temp:
            file.save(temp.name)
            wav_path, txt_path = process_data(temp.name)
            zip_path = os.path.join(tempfile.gettempdir(), f"{Path(temp.name).stem}.zip")
            with zipfile.ZipFile(zip_path, 'w') as zipf:
                zipf.write(wav_path, os.path.basename(wav_path))
                zipf.write(txt_path, os.path.basename(txt_path))
            return send_file(zip_path, as_attachment=True)
    except Exception as e:
        logger.error(f"Error processing request: {str(e)}")
        return jsonify({"status": "error", "message": str(e)}), 400

if __name__ == '__main__':
    serve(app, host='0.0.0.0', port=5000)