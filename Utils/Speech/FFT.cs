using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Alfred.Utils.Ressources;
using MathNet.Numerics.Transformations;
using Microsoft.Speech.Recognition;

namespace Alfred.Utils
{
	public class FFT
	{
		public Dictionary<double, double> fft { get; set; }
		public CWave Wave { get; set; }
        public double[] Left { get; set; }
        public double[] Right { get; set; }

		private int l;
		private Stream output;
        
		public void Transform()
		{
			Left = new double[l];
			Right = new double[l];

			for (var i = 0; i < l; i++)
			{
				Left[i] = Wave.waveData.Samples16Bit[i].Left;
				Right[i] = Wave.waveData.Samples16Bit[i].Right;
			}
			var real = new double[l];
			var im = new double[l];

			var rft = new RealFourierTransformation();
			rft.TransformForward(Left, out real, out im);

			var fscale = rft.GenerateFrequencyScale(Wave.waveHeader.FREQ, l);

            fft = new Dictionary<double, double>();
			for (var i = 0; i < l/2; i++)
			{
                var val = Math.Log(Math.Pow(real[i], 2) + Math.Pow(im[i], 2)) * 10;
                if (fscale[i] < 100)
                    fft.Add(fscale[i], 0);
                else
                    fft.Add(fscale[i], val);
			}
		}

		public void OrderFFT()
		{
			fft = fft.OrderByDescending(p => p.Value).ToDictionary(p=>p.Key, p=>p.Value);
		}

		private double getFrequency()
		{			
			Transform();
			OrderFFT();
			return fft.First().Key;
		}

		private double getEnergy()
		{
			double e = 0;
			for (var i = 0; i < Left.Length; i++)
				e = e + (Left[i] * Left[i] / 1000000000);

			return e;
		}

		public bool isSignalOk(RecognizedAudio audio, DateTime start, float confidence)
		{
            saveRecognizedAudio(audio, start);

            Wave = new CWave(Paths.path_temp_wav);
            l = (int)Math.Pow(2, (int)Math.Floor(Math.Log(Wave.waveData.Samples16Bit.Length) / Math.Log(2)));
		
			var F = getFrequency();
			var E = getEnergy();
			return (F > 100 && F < 1000 && confidence > 0.60 && E > 1);
		}
		
		private void saveRecognizedAudio(RecognizedAudio audio, DateTime d)
		{
			var start = new TimeSpan(d.Day, d.Hour, d.Minute, d.Second, d.Millisecond);
			var duration = audio.Duration;

			output = new FileStream(Paths.path_temp_wav, FileMode.Create);
			var nameAudio = audio.GetRange(new TimeSpan(0), duration);
			nameAudio.WriteToWaveStream(output);
			output.Close();
		}
	}
}
