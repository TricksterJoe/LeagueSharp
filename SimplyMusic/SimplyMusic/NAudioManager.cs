using System;
using NAudio.Wave;

namespace SimplyMusic
{
    class NAudioManager
    {
        private static IWavePlayer _wavePlayer;  
        private static WaveStream _waveStream;     
        private static WaveChannel32 _waveVolume;

        public enum PlayBackState    
        {
            Stopped = 0,
            Playing = 1,
            Paused = 2,
        }

        public static bool PlayAudio(string fileName)
        {
            if (!System.IO.File.Exists(fileName)) return false;

            _wavePlayer = new WaveOut();

            try
            {
                _waveStream = CreateInputStream(fileName);               
                _wavePlayer.Init(_waveStream);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public TimeSpan TotalDuration
        {
            get {
                return _waveStream != null ? _waveStream.TotalTime : new TimeSpan();
            }
        }

        public string TotalTime
        {
            get {
                return _waveStream != null ? String.Format("{0:00}:{1:00}", (int)_waveStream.TotalTime.TotalMinutes, _waveStream.TotalTime.Seconds) : "00:00";
            }
        }

        public TimeSpan TimePosition
        {
            get
            {
                return _waveStream == null ? TimeSpan.Zero : _waveStream.CurrentTime;
            }
            set { if (_waveStream != null) _waveStream.CurrentTime = value; }
        }

        public void Pause()
        {
            if (_wavePlayer == null) return;
            _wavePlayer.Pause();
        }

        public void Play()
        {

            if (_wavePlayer == null) return;

            if (_wavePlayer.PlaybackState == PlaybackState.Playing) return;

            _wavePlayer.Play();

        }

        public void Stop()
        {
            _wavePlayer.Stop();
        }

        public PlayBackState State
        {
            get {
                return _wavePlayer != null
                    ? (PlayBackState) _wavePlayer.PlaybackState
                    : PlayBackState.Stopped;
            }
        }

        public float Volume
        {
            get { return _waveVolume.Volume; }
            set
            {
                if (_waveVolume == null) return;
                _waveVolume.Volume = value;
            }
        }

        private static WaveStream CreateInputStream(string fileName)
        {
            var reader = CreateReaderStream(fileName);
            _waveVolume = new WaveChannel32(reader);
            return _waveVolume;
        }

        private static WaveStream CreateReaderStream(string fileName)
        {
            WaveStream readerStream = null;
            if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                readerStream = new WaveFileReader(fileName);
                if (readerStream.WaveFormat.Encoding == WaveFormatEncoding.Pcm ||
                    readerStream.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat) return readerStream;
                readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
                readerStream = new BlockAlignReductionStream(readerStream);
            }
            else if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                readerStream = new Mp3FileReader(fileName);
            
            else if (fileName.EndsWith(".aiff"))
                readerStream = new AiffFileReader(fileName);
            
            return readerStream;
        }
    }
}

