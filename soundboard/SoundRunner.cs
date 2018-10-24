using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace soundboard
{
	public class SoundRunner
	{
		private static WaveOutEvent outputDevice;
		private AudioFileReader audioFile;
		private bool isOnGoing = false;

		private SoundRunner() { }


		public static SoundRunner Instance
		{
			get { return instance; }
		}

		public int deviceNumber = -2;
		private static SoundRunner instance = new SoundRunner();

		public void PlaySound(string song=null)
		{
			if (isOnGoing || song == null) 
			{
				outputDevice?.Stop();
			}
			else
			{
				Play(song);
			}
			isOnGoing = !isOnGoing;
		}
		private void Play(string song = null)
		{
			//if (song != null) pathToFile = song;
			if (outputDevice == null)
			{
				outputDevice = new WaveOutEvent() { DeviceNumber = deviceNumber };
				outputDevice.PlaybackStopped += OnPlaybackStopped;
			}

			if (audioFile == null)
			{
				audioFile = new AudioFileReader(song);
				outputDevice.Init(audioFile);
			}

			outputDevice.Play();
		}

		public void Stop()
		{
			outputDevice?.Stop();
		}

		private void OnPlaybackStopped(object sender, StoppedEventArgs args)
		{
			outputDevice.Dispose();
			outputDevice = null;
			audioFile.Dispose();
			audioFile = null;
		}
	}
}
