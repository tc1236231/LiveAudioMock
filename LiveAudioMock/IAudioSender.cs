using System;

namespace LiveAudioMock
{
    interface IAudioSender : IDisposable
    {
        void Send(byte[] payload);
    }
}
