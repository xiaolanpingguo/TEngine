using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lockstep.Framework;
using TEngine;


namespace Lockstep.Game
{
    public class MemoryBuffer : MemoryStream, IBufferWriter<byte>
    {
        private int _origin;

        public MemoryBuffer()
        {
        }

        public MemoryBuffer(int capacity) : base(capacity)
        {
        }

        public MemoryBuffer(byte[] buffer) : base(buffer)
        {
        }

        public MemoryBuffer(byte[] buffer, int index, int length) : base(buffer, index, length)
        {
            this._origin = index;
        }

        public ReadOnlyMemory<byte> WrittenMemory => this.GetBuffer().AsMemory(this._origin, (int)this.Position);

        public ReadOnlySpan<byte> WrittenSpan => this.GetBuffer().AsSpan(this._origin, (int)this.Position);

        public void Advance(int count)
        {
            long newLength = this.Position + count;
            if (newLength > this.Length)
            {
                this.SetLength(newLength);
            }
            this.Position = newLength;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (this.Length - this.Position < sizeHint)
            {
                this.SetLength(this.Position + sizeHint);
            }
            var memory = this.GetBuffer().AsMemory((int)this.Position + this._origin, (int)(this.Length - this.Position));
            return memory;
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            if (this.Length - this.Position < sizeHint)
            {
                this.SetLength(this.Position + sizeHint);
            }
            var span = this.GetBuffer().AsSpan((int)this.Position + this._origin, (int)(this.Length - this.Position));
            return span;
        }
    }

    public class FrameBuffer
    {
        private readonly int _frameRate;
        private int _maxFrame;
        private readonly List<ServerFrame> _frameInputs;
        private readonly List<MemoryBuffer> _snapshots;
        private readonly List<long> _hashs;

        public FrameBuffer(int frame = 0, int frameRate = 20)
        {
            _frameRate = frameRate;
            _maxFrame = frame + frameRate * 30;
            int capacity = frameRate * 60;
            _frameInputs = new List<ServerFrame>(capacity);
            _snapshots = new List<MemoryBuffer>(capacity);
            _hashs = new List<long>(capacity);

            for (int i = 0; i < _snapshots.Capacity; ++i)
            {
                _hashs.Add(0);
                _frameInputs.Add(new ServerFrame());
                MemoryBuffer memoryBuffer = new(10240);
                memoryBuffer.SetLength(0);
                memoryBuffer.Seek(0, SeekOrigin.Begin);
                _snapshots.Add(memoryBuffer);
            }
        }

        public void SetHash(int frame, long hash)
        {
            EnsureFrame(frame);
            _hashs[frame % _frameInputs.Capacity] = hash;
        }

        public long GetHash(int frame)
        {
            EnsureFrame(frame);
            return _hashs[frame % _frameInputs.Capacity];
        }

        public bool CheckFrame(int frame)
        {
            if (frame < 0)
            {
                return false;
            }

            if (frame > _maxFrame)
            {
                return false;
            }

            return true;
        }

        private void EnsureFrame(int frame)
        {
            if (!CheckFrame(frame))
            {
                throw new Exception($"frame out: {frame}, maxframe: {_maxFrame}");
            }
        }

        public ServerFrame GetFrame(int frame)
        {
            EnsureFrame(frame);
            ServerFrame serverFrame = _frameInputs[frame % _frameInputs.Capacity];
            return serverFrame;
        }

        public void MoveForward(int frame)
        {
            // at least reserve 1s
            if (_maxFrame - frame > _frameRate)
            {
                return;
            }

            ++_maxFrame;

            ServerFrame serverFrame = GetFrame(_maxFrame);
            //serverFrame.Inputs.Clear();
        }

        public MemoryBuffer Snapshot(int frame)
        {
            EnsureFrame(frame);
            MemoryBuffer memoryBuffer = _snapshots[frame % _snapshots.Capacity];
            return memoryBuffer;
        }
    }
}