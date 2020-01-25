using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveAudioMock
{
    public partial class Form1 : Form
    {
        private bool mPlaying = true;
        private BufferedWaveProvider buffer;
        private WaveOut mAudioTrack;

        public Form1()
        {
            InitializeComponent();
            var waveFormat = new WaveFormat(16000, 16, 1);
            buffer = new BufferedWaveProvider(waveFormat)
            {
                BufferDuration = TimeSpan.FromSeconds(10),
                DiscardOnBufferOverflow = false
            };
            ConfigureCodec();


            Task.Run(() =>
            {
                using (var reader = new WaveFileReader(@"C:\Users\tc1236231\Desktop\CabinBroadcast\SafetyInstruction.wav"))
                {
                    var newFormat = waveFormat;
                    using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                    {
                        using (var loopStream = new LoopStream(conversionStream))
                        {
                            var startTime = DateTime.Now;
                            while (true)
                            {

                                var buffer = new byte[1024];
                                loopStream.Read(buffer, 0, buffer.Length);

                                var currentTime = DateTime.Now;
                                var ts1 = new TimeSpan(startTime.Ticks);
                                var ts2 = new TimeSpan(currentTime.Ticks);
                                var ts = ts2 - ts1;
                                var audioTs = loopStream.CurrentTime;

                                Console.WriteLine(audioTs - ts);

                                OnDataReceived(buffer);
                                Thread.Sleep(Convert.ToInt32((audioTs - ts).TotalMilliseconds));
                            }
                        }
                    }
                }
            });
        }

        internal void OnDataReceived(byte[] currentFrame)
        {
            if (mPlaying && mAudioTrack != null)
            {
                buffer.AddSamples(currentFrame, 0, currentFrame.Length);
            }
        }

        internal void ConfigureCodec()
        {
            mAudioTrack = new WaveOut();
            mAudioTrack.Init(buffer);
            if (mPlaying)
            {
                mAudioTrack.Play();
            }
        }

        private WaveOut waveOut;

        private void button1_Click(object sender, EventArgs e)
        {
            ConfigureCodec();
        }
    }
}
