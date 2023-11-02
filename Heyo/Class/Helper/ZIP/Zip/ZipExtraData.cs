//
// ZipExtraData.cs
//
// Copyright 2004-2007 John Reilly
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// Linking this library statically or dynamically with other modules is
// making a combined work based on this library.  Thus, the terms and
// conditions of the GNU General Public License cover the whole
// combination.
// 
// As a special exception, the copyright holders of this library give you
// permission to link this library with independent modules to produce an
// executable, regardless of the license terms of these independent
// modules, and to copy and distribute the resulting executable under
// terms of your choice, provided that you also meet, for each linked
// independent module, the terms and conditions of the license of that
// module.  An independent module is a module which is not derived from
// or based on this library.  If you modify this library, you may extend
// this exception to your version of the library, but you are not
// obligated to do so.  If you do not wish to do so, delete this
// exception statement from your version.

using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
    // TODO: Sort out wether tagged data is useful and what a good implementation might look like.
    // Its just a sketch of an idea at the moment.

    /// <summary>
    ///     ExtraData tagged value interface.
    /// </summary>
    public interface ITaggedData
    {
        /// <summary>
        ///     Get the ID for this tagged data value.
        /// </summary>
        short TagID { get; }

        /// <summary>
        ///     Set the contents of this instance from the data passed.
        /// </summary>
        /// <param name="data">The data to extract contents from.</param>
        /// <param name="offset">The offset to begin extracting data from.</param>
        /// <param name="count">The number of bytes to extract.</param>
        void SetData(byte[] data, int offset, int count);

        /// <summary>
        ///     Get the data representing this instance.
        /// </summary>
        /// <returns>Returns the data for this instance.</returns>
        byte[] GetData();
    }

    /// <summary>
    ///     A raw binary tagged value
    /// </summary>
    public class RawTaggedData : ITaggedData
    {
        #region Instance Fields

        /// <summary>
        ///     The tag ID for this instance.
        /// </summary>
        private short _tag;

        #endregion

        /// <summary>
        ///     Initialise a new instance.
        /// </summary>
        /// <param name="tag">The tag ID.</param>
        public RawTaggedData(short tag)
        {
            _tag = tag;
        }

        /// <summary>
        ///     Get /set the binary data representing this instance.
        /// </summary>
        /// <returns>The raw binary data representing this instance.</returns>
        public byte[] Data { get; set; }

        #region ITaggedData Members

        /// <summary>
        ///     Get the ID for this tagged data value.
        /// </summary>
        public short TagID
        {
            get => _tag;
            set => _tag = value;
        }

        /// <summary>
        ///     Set the data from the raw values provided.
        /// </summary>
        /// <param name="data">The raw data to extract values from.</param>
        /// <param name="offset">The index to start extracting values from.</param>
        /// <param name="count">The number of bytes available.</param>
        public void SetData(byte[] data, int offset, int count)
        {
            if (data == null) throw new ArgumentNullException("data");

            Data = new byte[count];
            Array.Copy(data, offset, Data, 0, count);
        }

        /// <summary>
        ///     Get the binary data representing this instance.
        /// </summary>
        /// <returns>The raw binary data representing this instance.</returns>
        public byte[] GetData()
        {
            return Data;
        }

        #endregion
    }

    /// <summary>
    ///     Class representing extended unix date time values.
    /// </summary>
    public class ExtendedUnixData : ITaggedData
    {
        /// <summary>
        ///     Flags indicate which values are included in this instance.
        /// </summary>
        [Flags]
        public enum Flags : byte
        {
            /// <summary>
            ///     The modification time is included
            /// </summary>
            ModificationTime = 0x01,

            /// <summary>
            ///     The access time is included
            /// </summary>
            AccessTime = 0x02,

            /// <summary>
            ///     The create time is included.
            /// </summary>
            CreateTime = 0x04
        }

        /// <summary>
        ///     Get /set the Modification Time
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <seealso cref="IsValidValue"></seealso>
        public DateTime ModificationTime
        {
            get => _modificationTime;
            set
            {
                if (!IsValidValue(value)) throw new ArgumentOutOfRangeException("value");

                Include |= Flags.ModificationTime;
                _modificationTime = value;
            }
        }

        /// <summary>
        ///     Get / set the Access Time
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <seealso cref="IsValidValue"></seealso>
        public DateTime AccessTime
        {
            get => _lastAccessTime;
            set
            {
                if (!IsValidValue(value)) throw new ArgumentOutOfRangeException("value");

                Include |= Flags.AccessTime;
                _lastAccessTime = value;
            }
        }

        /// <summary>
        ///     Get / Set the Create Time
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <seealso cref="IsValidValue"></seealso>
        public DateTime CreateTime
        {
            get => _createTime;
            set
            {
                if (!IsValidValue(value)) throw new ArgumentOutOfRangeException("value");

                Include |= Flags.CreateTime;
                _createTime = value;
            }
        }

        /// <summary>
        ///     Get/set the <see cref="Flags">values</see> to include.
        /// </summary>
        private Flags Include { get; set; }

        /// <summary>
        ///     Test a <see cref="DateTime"> value to see if is valid and can be represented here.</see>
        /// </summary>
        /// <param name="value">The <see cref="DateTime">value</see> to test.</param>
        /// <returns>Returns true if the value is valid and can be represented; false if not.</returns>
        /// <remarks>
        ///     The standard Unix time is a signed integer data type, directly encoding the Unix time number,
        ///     which is the number of seconds since 1970-01-01.
        ///     Being 32 bits means the values here cover a range of about 136 years.
        ///     The minimum representable time is 1901-12-13 20:45:52,
        ///     and the maximum representable time is 2038-01-19 03:14:07.
        /// </remarks>
        public static bool IsValidValue(DateTime value)
        {
            return value >= new DateTime(1901, 12, 13, 20, 45, 52) ||
                   value <= new DateTime(2038, 1, 19, 03, 14, 07);
        }

        #region ITaggedData Members

        /// <summary>
        ///     Get the ID
        /// </summary>
        public short TagID => 0x5455;

        /// <summary>
        ///     Set the data from the raw values provided.
        /// </summary>
        /// <param name="data">The raw data to extract values from.</param>
        /// <param name="index">The index to start extracting values from.</param>
        /// <param name="count">The number of bytes available.</param>
        public void SetData(byte[] data, int index, int count)
        {
            using var ms = new MemoryStream(data, index, count, false);
            using var helperStream = new ZipHelperStream(ms);
            // bit 0           if set, modification time is present
            // bit 1           if set, access time is present
            // bit 2           if set, creation time is present

            Include = (Flags) helperStream.ReadByte();
            if ((Include & Flags.ModificationTime) != 0 && count >= 5)
            {
                var iTime = helperStream.ReadLEInt();

                _modificationTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() +
                                     new TimeSpan(0, 0, 0, iTime, 0)).ToLocalTime();
            }

            if ((Include & Flags.AccessTime) != 0)
            {
                var iTime = helperStream.ReadLEInt();

                _lastAccessTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() +
                                   new TimeSpan(0, 0, 0, iTime, 0)).ToLocalTime();
            }

            if ((Include & Flags.CreateTime) != 0)
            {
                var iTime = helperStream.ReadLEInt();

                _createTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() +
                               new TimeSpan(0, 0, 0, iTime, 0)).ToLocalTime();
            }
        }

        /// <summary>
        ///     Get the binary data representing this instance.
        /// </summary>
        /// <returns>The raw binary data representing this instance.</returns>
        public byte[] GetData()
        {
            using var ms = new MemoryStream();
            using var helperStream = new ZipHelperStream(ms);
            helperStream.IsStreamOwner = false;
            helperStream.WriteByte((byte) Include); // Flags
            if ((Include & Flags.ModificationTime) != 0)
            {
                var span = _modificationTime.ToUniversalTime() -
                           new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime();
                var seconds = (int) span.TotalSeconds;
                helperStream.WriteLEInt(seconds);
            }

            if ((Include & Flags.AccessTime) != 0)
            {
                var span = _lastAccessTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime();
                var seconds = (int) span.TotalSeconds;
                helperStream.WriteLEInt(seconds);
            }

            if ((Include & Flags.CreateTime) != 0)
            {
                var span = _createTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime();
                var seconds = (int) span.TotalSeconds;
                helperStream.WriteLEInt(seconds);
            }

            return ms.ToArray();
        }

        #endregion

        #region Instance Fields

        private DateTime _modificationTime = new DateTime(1970, 1, 1);
        private DateTime _lastAccessTime = new DateTime(1970, 1, 1);
        private DateTime _createTime = new DateTime(1970, 1, 1);

        #endregion
    }

    /// <summary>
    ///     Class handling NT date time values.
    /// </summary>
    public class NTTaggedData : ITaggedData
    {
        /// <summary>
        ///     Get/set the <see cref="DateTime">last modification time</see>.
        /// </summary>
        public DateTime LastModificationTime
        {
            get => _lastModificationTime;
            set
            {
                if (!IsValidValue(value)) throw new ArgumentOutOfRangeException("value");
                _lastModificationTime = value;
            }
        }

        /// <summary>
        ///     Get /set the <see cref="DateTime">create time</see>
        /// </summary>
        public DateTime CreateTime
        {
            get => _createTime;
            set
            {
                if (!IsValidValue(value)) throw new ArgumentOutOfRangeException("value");
                _createTime = value;
            }
        }

        /// <summary>
        ///     Get /set the <see cref="DateTime">last access time</see>.
        /// </summary>
        public DateTime LastAccessTime
        {
            get => _lastAccessTime;
            set
            {
                if (!IsValidValue(value)) throw new ArgumentOutOfRangeException("value");
                _lastAccessTime = value;
            }
        }

        /// <summary>
        ///     Get the ID for this tagged data value.
        /// </summary>
        public short TagID => 10;

        /// <summary>
        ///     Set the data from the raw values provided.
        /// </summary>
        /// <param name="data">The raw data to extract values from.</param>
        /// <param name="index">The index to start extracting values from.</param>
        /// <param name="count">The number of bytes available.</param>
        public void SetData(byte[] data, int index, int count)
        {
            using var ms = new MemoryStream(data, index, count, false);
            using var helperStream = new ZipHelperStream(ms);
            helperStream.ReadLEInt(); // Reserved
            while (helperStream.Position < helperStream.Length)
            {
                var ntfsTag = helperStream.ReadLEShort();
                var ntfsLength = helperStream.ReadLEShort();
                if (ntfsTag == 1)
                {
                    if (ntfsLength >= 24)
                    {
                        var lastModificationTicks = helperStream.ReadLELong();
                        _lastModificationTime = DateTime.FromFileTime(lastModificationTicks);

                        var lastAccessTicks = helperStream.ReadLELong();
                        _lastAccessTime = DateTime.FromFileTime(lastAccessTicks);

                        var createTimeTicks = helperStream.ReadLELong();
                        _createTime = DateTime.FromFileTime(createTimeTicks);
                    }

                    break;
                }

                // An unknown NTFS tag so simply skip it.
                helperStream.Seek(ntfsLength, SeekOrigin.Current);
            }
        }

        /// <summary>
        ///     Get the binary data representing this instance.
        /// </summary>
        /// <returns>The raw binary data representing this instance.</returns>
        public byte[] GetData()
        {
            using var ms = new MemoryStream();
            using var helperStream = new ZipHelperStream(ms);
            helperStream.IsStreamOwner = false;
            helperStream.WriteLEInt(0); // Reserved
            helperStream.WriteLEShort(1); // Tag
            helperStream.WriteLEShort(24); // Length = 3 x 8.
            helperStream.WriteLELong(_lastModificationTime.ToFileTime());
            helperStream.WriteLELong(_lastAccessTime.ToFileTime());
            helperStream.WriteLELong(_createTime.ToFileTime());
            return ms.ToArray();
        }

        /// <summary>
        ///     Test a <see cref="DateTime"> valuie to see if is valid and can be represented here.</see>
        /// </summary>
        /// <param name="value">The <see cref="DateTime">value</see> to test.</param>
        /// <returns>Returns true if the value is valid and can be represented; false if not.</returns>
        /// <remarks>
        ///     NTFS filetimes are 64-bit unsigned integers, stored in Intel
        ///     (least significant byte first) byte order. They determine the
        ///     number of 1.0E-07 seconds (1/10th microseconds!) past WinNT "epoch",
        ///     which is "01-Jan-1601 00:00:00 UTC". 28 May 60056 is the upper limit
        /// </remarks>
        public static bool IsValidValue(DateTime value)
        {
            var result = true;
            try
            {
                value.ToFileTimeUtc();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #region Instance Fields

        private DateTime _lastAccessTime = DateTime.FromFileTime(0);
        private DateTime _lastModificationTime = DateTime.FromFileTime(0);
        private DateTime _createTime = DateTime.FromFileTime(0);

        #endregion
    }

    /// <summary>
    ///     A factory that creates <see cref="ITaggedData">tagged data</see> instances.
    /// </summary>
    internal interface ITaggedDataFactory
    {
        /// <summary>
        ///     Get data for a specific tag value.
        /// </summary>
        /// <param name="tag">The tag ID to find.</param>
        /// <param name="data">The data to search.</param>
        /// <param name="offset">The offset to begin extracting data from.</param>
        /// <param name="count">The number of bytes to extract.</param>
        /// <returns>The located <see cref="ITaggedData">value found</see>, or null if not found.</returns>
        ITaggedData Create(short tag, byte[] data, int offset, int count);
    }

    /// <summary>
    ///     A class to handle the extra data field for Zip entries
    /// </summary>
    /// <remarks>
    ///     Extra data contains 0 or more values each prefixed by a header tag and length.
    ///     They contain zero or more bytes of actual data.
    ///     The data is held internally using a copy on write strategy.  This is more efficient but
    ///     means that for extra data created by passing in data can have the values modified by the caller
    ///     in some circumstances.
    /// </remarks>
    public sealed class ZipExtraData : IDisposable
    {
        /// <summary>
        ///     Gets the current extra data length.
        /// </summary>
        public int Length => _data.Length;

        /// <summary>
        ///     Get the length of the last value found by <see cref="Find" />
        /// </summary>
        /// <remarks>This is only valid if <see cref="Find" /> has previously returned true.</remarks>
        public int ValueLength { get; private set; }

        /// <summary>
        ///     Get the index for the current read value.
        /// </summary>
        /// <remarks>
        ///     This is only valid if <see cref="Find" /> has previously returned true.
        ///     Initially the result will be the index of the first byte of actual data.  The value is updated after calls to
        ///     <see cref="ReadInt" />, <see cref="ReadShort" /> and <see cref="ReadLong" />.
        /// </remarks>
        public int CurrentReadIndex { get; private set; }

        /// <summary>
        ///     Get the number of bytes remaining to be read for the current value;
        /// </summary>
        public int UnreadCount
        {
            get
            {
                if (_readValueStart > _data.Length ||
                    _readValueStart < 4)
                    throw new ZipException("Find must be called before calling a Read method");

                return _readValueStart + ValueLength - CurrentReadIndex;
            }
        }

        #region IDisposable Members

        /// <summary>
        ///     Dispose of this instance.
        /// </summary>
        public void Dispose()
        {
            if (_newEntry != null) _newEntry.Close();
        }

        #endregion

        /// <summary>
        ///     Get the raw extra data value
        /// </summary>
        /// <returns>Returns the raw byte[] extra data this instance represents.</returns>
        public byte[] GetEntryData()
        {
            if (Length > ushort.MaxValue) throw new ZipException("Data exceeds maximum length");

            return (byte[]) _data.Clone();
        }

        /// <summary>
        ///     Clear the stored data.
        /// </summary>
        public void Clear()
        {
            if (_data == null || _data.Length != 0) _data = new byte[0];
        }

        /// <summary>
        ///     Get a read-only <see cref="Stream" /> for the associated tag.
        /// </summary>
        /// <param name="tag">The tag to locate data for.</param>
        /// <returns>Returns a <see cref="Stream" /> containing tag data or null if no tag was found.</returns>
        public Stream GetStreamForTag(int tag)
        {
            Stream result = null;
            if (Find(tag)) result = new MemoryStream(_data, CurrentReadIndex, ValueLength, false);
            return result;
        }

        /// <summary>
        ///     Get the <see cref="ITaggedData">tagged data</see> for a tag.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>Returns a <see cref="ITaggedData">tagged value</see> or null if none found.</returns>
        private ITaggedData GetData(short tag)
        {
            ITaggedData result = null;
            if (Find(tag)) result = Create(tag, _data, _readValueStart, ValueLength);
            return result;
        }

        private static ITaggedData Create(short tag, byte[] data, int offset, int count)
        {
            ITaggedData result = null;
            switch (tag)
            {
                case 0x000A:
                    result = new NTTaggedData();
                    break;
                case 0x5455:
                    result = new ExtendedUnixData();
                    break;
                default:
                    result = new RawTaggedData(tag);
                    break;
            }

            result.SetData(data, offset, count);
            return result;
        }

        /// <summary>
        ///     Find an extra data value
        /// </summary>
        /// <param name="headerID">The identifier for the value to find.</param>
        /// <returns>Returns true if the value was found; false otherwise.</returns>
        public bool Find(int headerID)
        {
            _readValueStart = _data.Length;
            ValueLength = 0;
            CurrentReadIndex = 0;

            var localLength = _readValueStart;
            var localTag = headerID - 1;

            // Trailing bytes that cant make up an entry (as there arent enough
            // bytes for a tag and length) are ignored!
            while (localTag != headerID && CurrentReadIndex < _data.Length - 3)
            {
                localTag = ReadShortInternal();
                localLength = ReadShortInternal();
                if (localTag != headerID) CurrentReadIndex += localLength;
            }

            var result = localTag == headerID && CurrentReadIndex + localLength <= _data.Length;

            if (result)
            {
                _readValueStart = CurrentReadIndex;
                ValueLength = localLength;
            }

            return result;
        }

        /// <summary>
        ///     Add a new entry to extra data.
        /// </summary>
        /// <param name="taggedData">The <see cref="ITaggedData" /> value to add.</param>
        public void AddEntry(ITaggedData taggedData)
        {
            if (taggedData == null) throw new ArgumentNullException("taggedData");
            AddEntry(taggedData.TagID, taggedData.GetData());
        }

        /// <summary>
        ///     Add a new entry to extra data
        /// </summary>
        /// <param name="headerID">The ID for this entry.</param>
        /// <param name="fieldData">The data to add.</param>
        /// <remarks>If the ID already exists its contents are replaced.</remarks>
        public void AddEntry(int headerID, byte[] fieldData)
        {
            if (headerID > ushort.MaxValue || headerID < 0) throw new ArgumentOutOfRangeException("headerID");

            var addLength = fieldData == null ? 0 : fieldData.Length;

            if (addLength > ushort.MaxValue)
            {
#if NETCF_1_0
				throw new ArgumentOutOfRangeException("fieldData");
#else
                throw new ArgumentOutOfRangeException("fieldData", "exceeds maximum length");
#endif
            }

            // Test for new length before adjusting data.
            var newLength = _data.Length + addLength + 4;

            if (Find(headerID)) newLength -= ValueLength + 4;

            if (newLength > ushort.MaxValue) throw new ZipException("Data exceeds maximum length");

            Delete(headerID);

            var newData = new byte[newLength];
            _data.CopyTo(newData, 0);
            var index = _data.Length;
            _data = newData;
            SetShort(ref index, headerID);
            SetShort(ref index, addLength);
            if (fieldData != null) fieldData.CopyTo(newData, index);
        }

        /// <summary>
        ///     Start adding a new entry.
        /// </summary>
        /// <remarks>
        ///     Add data using <see cref="AddData(byte[])" />, <see cref="AddLeShort" />, <see cref="AddLeInt" />, or
        ///     <see cref="AddLeLong" />.
        ///     The new entry is completed and actually added by calling <see cref="AddNewEntry" />
        /// </remarks>
        /// <seealso cref="AddEntry(ITaggedData)" />
        public void StartNewEntry()
        {
            _newEntry = new MemoryStream();
        }

        /// <summary>
        ///     Add entry data added since <see cref="StartNewEntry" /> using the ID passed.
        /// </summary>
        /// <param name="headerID">The identifier to use for this entry.</param>
        public void AddNewEntry(int headerID)
        {
            var newData = _newEntry.ToArray();
            _newEntry = null;
            AddEntry(headerID, newData);
        }

        /// <summary>
        ///     Add a byte of data to the pending new entry.
        /// </summary>
        /// <param name="data">The byte to add.</param>
        /// <seealso cref="StartNewEntry" />
        public void AddData(byte data)
        {
            _newEntry.WriteByte(data);
        }

        /// <summary>
        ///     Add data to a pending new entry.
        /// </summary>
        /// <param name="data">The data to add.</param>
        /// <seealso cref="StartNewEntry" />
        public void AddData(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");

            _newEntry.Write(data, 0, data.Length);
        }

        /// <summary>
        ///     Add a short value in little endian order to the pending new entry.
        /// </summary>
        /// <param name="toAdd">The data to add.</param>
        /// <seealso cref="StartNewEntry" />
        public void AddLeShort(int toAdd)
        {
            unchecked
            {
                _newEntry.WriteByte((byte) toAdd);
                _newEntry.WriteByte((byte) (toAdd >> 8));
            }
        }

        /// <summary>
        ///     Add an integer value in little endian order to the pending new entry.
        /// </summary>
        /// <param name="toAdd">The data to add.</param>
        /// <seealso cref="StartNewEntry" />
        public void AddLeInt(int toAdd)
        {
            unchecked
            {
                AddLeShort((short) toAdd);
                AddLeShort((short) (toAdd >> 16));
            }
        }

        /// <summary>
        ///     Add a long value in little endian order to the pending new entry.
        /// </summary>
        /// <param name="toAdd">The data to add.</param>
        /// <seealso cref="StartNewEntry" />
        public void AddLeLong(long toAdd)
        {
            unchecked
            {
                AddLeInt((int) (toAdd & 0xffffffff));
                AddLeInt((int) (toAdd >> 32));
            }
        }

        /// <summary>
        ///     Delete an extra data field.
        /// </summary>
        /// <param name="headerID">The identifier of the field to delete.</param>
        /// <returns>Returns true if the field was found and deleted.</returns>
        public bool Delete(int headerID)
        {
            var result = false;

            if (Find(headerID))
            {
                result = true;
                var trueStart = _readValueStart - 4;

                var newData = new byte[_data.Length - (ValueLength + 4)];
                Array.Copy(_data, 0, newData, 0, trueStart);

                var trueEnd = trueStart + ValueLength + 4;
                Array.Copy(_data, trueEnd, newData, trueStart, _data.Length - trueEnd);
                _data = newData;
            }

            return result;
        }

        #region Constructors

        /// <summary>
        ///     Initialise a default instance.
        /// </summary>
        public ZipExtraData()
        {
            Clear();
        }

        /// <summary>
        ///     Initialise with known extra data.
        /// </summary>
        /// <param name="data">The extra data.</param>
        public ZipExtraData(byte[] data)
        {
            if (data == null)
                _data = new byte[0];
            else
                _data = data;
        }

        #endregion

        #region Reading Support

        /// <summary>
        ///     Read a long in little endian form from the last <see cref="Find">found</see> data value
        /// </summary>
        /// <returns>Returns the long value read.</returns>
        public long ReadLong()
        {
            ReadCheck(8);
            return (ReadInt() & 0xffffffff) | ((long) ReadInt() << 32);
        }

        /// <summary>
        ///     Read an integer in little endian form from the last <see cref="Find">found</see> data value.
        /// </summary>
        /// <returns>Returns the integer read.</returns>
        public int ReadInt()
        {
            ReadCheck(4);

            var result = _data[CurrentReadIndex] + (_data[CurrentReadIndex + 1] << 8) +
                         (_data[CurrentReadIndex + 2] << 16) + (_data[CurrentReadIndex + 3] << 24);
            CurrentReadIndex += 4;
            return result;
        }

        /// <summary>
        ///     Read a short value in little endian form from the last <see cref="Find">found</see> data value.
        /// </summary>
        /// <returns>Returns the short value read.</returns>
        public int ReadShort()
        {
            ReadCheck(2);
            var result = _data[CurrentReadIndex] + (_data[CurrentReadIndex + 1] << 8);
            CurrentReadIndex += 2;
            return result;
        }

        /// <summary>
        ///     Read a byte from an extra data
        /// </summary>
        /// <returns>The byte value read or -1 if the end of data has been reached.</returns>
        public int ReadByte()
        {
            var result = -1;
            if (CurrentReadIndex < _data.Length && _readValueStart + ValueLength > CurrentReadIndex)
            {
                result = _data[CurrentReadIndex];
                CurrentReadIndex += 1;
            }

            return result;
        }

        /// <summary>
        ///     Skip data during reading.
        /// </summary>
        /// <param name="amount">The number of bytes to skip.</param>
        public void Skip(int amount)
        {
            ReadCheck(amount);
            CurrentReadIndex += amount;
        }

        private void ReadCheck(int length)
        {
            if (_readValueStart > _data.Length ||
                _readValueStart < 4)
                throw new ZipException("Find must be called before calling a Read method");

            if (CurrentReadIndex > _readValueStart + ValueLength - length) throw new ZipException("End of extra data");

            if (CurrentReadIndex + length < 4) throw new ZipException("Cannot read before start of tag");
        }

        /// <summary>
        ///     Internal form of <see cref="ReadShort" /> that reads data at any location.
        /// </summary>
        /// <returns>Returns the short value read.</returns>
        private int ReadShortInternal()
        {
            if (CurrentReadIndex > _data.Length - 2) throw new ZipException("End of extra data");

            var result = _data[CurrentReadIndex] + (_data[CurrentReadIndex + 1] << 8);
            CurrentReadIndex += 2;
            return result;
        }

        private void SetShort(ref int index, int source)
        {
            _data[index] = (byte) source;
            _data[index + 1] = (byte) (source >> 8);
            index += 2;
        }

        #endregion

        #region Instance Fields

        private int _readValueStart;

        private MemoryStream _newEntry;
        private byte[] _data;

        #endregion
    }
}