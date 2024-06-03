using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class MusicModel
{
    private float timeSpawn;
    private List<Tuple<float, int>> notes;
    private AudioClip clip;

    private float songPosition;
    private float musicTime;
    private int nextIndex;

    public float TimeSpawn
    {
        get => timeSpawn;
        set => timeSpawn = value;
    }

    public List<Tuple<float, int>> Notes
    {
        get => notes;
        private set => notes = value;
    }

    public AudioClip Clip
    {
        get => clip;
        set => clip = value;
    }

    public float SongPosition
    {
        get => songPosition;
        set => songPosition = value;
    }

    public float MusicTime
    {
        get => musicTime;
        set => musicTime = value;
    }

    public int NextIndex
    {
        get => nextIndex;
        set => nextIndex = value;
    }

    public MusicModel(float timeSpawn, string notesFilePath, string audioClipName)
    {
        this.timeSpawn = timeSpawn;
        this.notes = LoadNotes(notesFilePath);
        this.clip = LoadAudioClip(audioClipName);
        this.nextIndex = 0;
    }

    private List<Tuple<float, int>> LoadNotes(string filePath)
    {
        List<Tuple<float, int>> loadedNotes = new List<Tuple<float, int>>();

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                bool readingNotes = false;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("[INFO]"))
                    {
                        string sourceLine = reader.ReadLine();
                        int startIndex = sourceLine.IndexOf("Source: ") + "Source: ".Length;
                        int endIndex = sourceLine.LastIndexOf('.');
                        string source = sourceLine.Substring(startIndex, endIndex - startIndex);
                    }
                    else if (line.Contains("[NOTES]"))
                    {
                        readingNotes = true;
                    }
                    else if (readingNotes)
                    {
                        string[] parts = line.Trim().Split(' ');
                        float time = float.Parse(parts[0], CultureInfo.InvariantCulture);
                        int value = int.Parse(parts[1]);
                        loadedNotes.Add(new Tuple<float, int>(time, value));
                    }
                }
            }
        }
        return loadedNotes;
    }

    private AudioClip LoadAudioClip(string clipName)
    {
        return Resources.Load<AudioClip>(clipName);
    }
}