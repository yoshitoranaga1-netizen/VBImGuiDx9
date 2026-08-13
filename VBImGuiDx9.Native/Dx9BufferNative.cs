using System;
using Vortice.Direct3D9;

namespace VBImGuiDx9.Native
{
    /// <summary>
    /// Low-level Direct3D9 buffer operations used by the VB.NET backend.
    ///
    /// This class isolates the Span-based Vortice API from the VB.NET
    /// rendering layer.
    /// </summary>
    public static unsafe class Dx9BufferNative
    {
        /// <summary>
        /// Copies data from unmanaged memory into a Direct3D9 vertex buffer.
        /// </summary>
        public static void SetVertexBufferData(
            IDirect3DVertexBuffer9 buffer,
            IntPtr source,
            int sizeInBytes,
            LockFlags flags)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            if (sizeInBytes < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(sizeInBytes));

            if (sizeInBytes == 0)
                return;

            if (source == IntPtr.Zero)
                throw new ArgumentException(
                    "Source pointer must not be zero.",
                    nameof(source));

            Span<byte> destination =
                buffer.Lock<byte>(
                    0,
                    (uint)sizeInBytes,
                    flags);

            try
            {
                ReadOnlySpan<byte> sourceSpan =
                    new ReadOnlySpan<byte>(
                        (void*)source,
                        sizeInBytes);

                sourceSpan.CopyTo(destination);
            }
            finally
            {
                buffer.Unlock();
            }
        }

        /// <summary>
        /// Copies data from unmanaged memory into a Direct3D9 index buffer.
        /// </summary>
        public static void SetIndexBufferData(
            IDirect3DIndexBuffer9 buffer,
            IntPtr source,
            int sizeInBytes,
            LockFlags flags)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            if (sizeInBytes < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(sizeInBytes));

            if (sizeInBytes == 0)
                return;

            if (source == IntPtr.Zero)
                throw new ArgumentException(
                    "Source pointer must not be zero.",
                    nameof(source));

            Span<byte> destination =
                buffer.Lock<byte>(
                    0,
                    (uint)sizeInBytes,
                    flags);

            try
            {
                ReadOnlySpan<byte> sourceSpan =
                    new ReadOnlySpan<byte>(
                        (void*)source,
                        sizeInBytes);

                sourceSpan.CopyTo(destination);
            }
            finally
            {
                buffer.Unlock();
            }
        }
    }
}