//	Copyright (c) 2012 Calvin Rien
//        http://the.darktable.com
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
//  derived from Gregorio Zanon's script
//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734

using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace Samples.Whisper
{
	public static class SaveWav
	{
		private const int HeaderSize = 44;

		public static byte[] Save(string filename, AudioClip clip)
		{
			if (!filename.ToLower().EndsWith(".wav"))
			{
				filename += ".wav";
			}

			var filepath = Path.Combine(Application.persistentDataPath, filename);

			// Make sure directory exists if user is saving to sub dir.
			Directory.CreateDirectory(Path.GetDirectoryName(filepath) ?? string.Empty);

			using(var memoryStream = CreateEmpty())
			{
				ConvertAndWrite(memoryStream, clip);
				WriteHeader(memoryStream, clip);

				return memoryStream.GetBuffer();
			}
		}

		public static AudioClip TrimSilence(AudioClip clip, float min, Action handler = null)
		{
			var samples = new float[clip.samples];

			clip.GetData(samples, 0);

			return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency, handler: handler);
		}

        public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool stream = false, Action handler = null)
        {
            try
            {
                int i;

                // Trim leading silence
                for (i = 0; i < samples.Count; i++)
                {
                    if (Mathf.Abs(samples[i]) > min)
                        break;
                }

                if (i > 0)
                    samples.RemoveRange(0, i);

                // If the list is now empty, return a silent clip
                if (samples.Count == 0)
                    return AudioClip.Create("EmptyClip", 1, channels, hz, stream);

                // Trim trailing silence
                for (i = samples.Count - 1; i >= 0; i--)
                {
                    if (Mathf.Abs(samples[i]) > min)
                        break;
                }

                // Remove trailing silence safely
                if (i < samples.Count - 1)
                    samples.RemoveRange(i + 1, samples.Count - (i + 1));

                if (samples.Count == 0)
                    return AudioClip.Create("EmptyClip", 1, channels, hz, stream);

                // Create AudioClip
                var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, stream);
                clip.SetData(samples.ToArray(), 0);
                return clip;
            }
            catch (Exception ex)
            {
                handler?.Invoke();  // Call the handler if provided
                                      // Return a silent clip as fallback
                return AudioClip.Create("ErrorClip", 1, channels, hz, stream);
            }
        }


        static MemoryStream CreateEmpty()
		{
			var memoryStream = new MemoryStream();
			byte emptyByte = new byte();

			for (int i = 0; i < HeaderSize; i++) //preparing the header
			{
				memoryStream.WriteByte(emptyByte);
			}

			return memoryStream;
		}

		static void ConvertAndWrite(MemoryStream memoryStream, AudioClip clip)
		{
			var samples = new float[clip.samples];

			clip.GetData(samples, 0);

			Int16[] intData = new Int16[samples.Length];
			//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

			Byte[] bytesData = new Byte[samples.Length * 2];
			//bytesData array is twice the size of
			//dataSource array because a float converted in Int16 is 2 bytes.

			int rescaleFactor = 32767; //to convert float to Int16

			for (int i = 0; i < samples.Length; i++)
			{
				intData[i] = (short)(samples[i] * rescaleFactor);
				Byte[] byteArr = BitConverter.GetBytes(intData[i]);
				byteArr.CopyTo(bytesData, i * 2);
			}

			memoryStream.Write(bytesData, 0, bytesData.Length);
		}

		static void WriteHeader(MemoryStream memoryStream, AudioClip clip)
		{

			var hz = clip.frequency;
			var channels = clip.channels;
			var samples = clip.samples;

			memoryStream.Seek(0, SeekOrigin.Begin);

			Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
			memoryStream.Write(riff, 0, 4);

			Byte[] chunkSize = BitConverter.GetBytes(memoryStream.Length - 8);
			memoryStream.Write(chunkSize, 0, 4);

			Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
			memoryStream.Write(wave, 0, 4);

			Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
			memoryStream.Write(fmt, 0, 4);

			Byte[] subChunk1 = BitConverter.GetBytes(16);
			memoryStream.Write(subChunk1, 0, 4);

			// UInt16 two = 2;
			UInt16 one = 1;

			Byte[] audioFormat = BitConverter.GetBytes(one);
			memoryStream.Write(audioFormat, 0, 2);

			Byte[] numChannels = BitConverter.GetBytes(channels);
			memoryStream.Write(numChannels, 0, 2);

			Byte[] sampleRate = BitConverter.GetBytes(hz);
			memoryStream.Write(sampleRate, 0, 4);

			Byte[]
				byteRate = BitConverter.GetBytes(hz * channels *
				                                 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
			memoryStream.Write(byteRate, 0, 4);

			UInt16 blockAlign = (ushort)(channels * 2);
			memoryStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

			UInt16 bps = 16;
			Byte[] bitsPerSample = BitConverter.GetBytes(bps);
			memoryStream.Write(bitsPerSample, 0, 2);

			Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
			memoryStream.Write(datastring, 0, 4);

			Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
			memoryStream.Write(subChunk2, 0, 4);

			// fileStream.Close();
		}
	}
}