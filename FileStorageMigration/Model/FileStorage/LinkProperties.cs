using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace FileStorageMigration.Model.FileStorage
{
    public class Properties
    {
        internal readonly Dictionary<string, string> _dict;

        internal Properties(Dictionary<string, string> dict)
        {
            _dict = dict;
        }

        public Properties() : this(new Dictionary<string, string>())
        {

        }

        public string this[string key]
        {
            get
            {
                if (_dict.TryGetValue(key, out var result))
                    return result;

                return null;
            }
            set
            {

                if (value == null)
                {
                    _dict.Remove(key);

                }
                else
                {
                    _dict[key] = value;
                }

            }
        }
    }
    public class LinkProperties
    {
        private readonly Properties _properties;
        public LinkProperties(Properties properties)
        {
            _properties = properties;
        }

        public LinkType Type
        {
            get => Enum.TryParse(_properties[nameof(Type)], out LinkType type)
                ? type
                : LinkType.None;
            set => _properties[nameof(Type)] = value.ToString();
        }

        public bool CheckPermission
        {
            get => _properties[nameof(CheckPermission)] == "1";
            set => _properties[nameof(CheckPermission)] = value ? "1" : null;
        }

        public int DriveId
        {
            get => int.Parse(_properties[nameof(DriveId)], CultureInfo.InvariantCulture);

            set => _properties[nameof(DriveId)] = value.ToString(CultureInfo.InvariantCulture);
        }
        public int ItemId
        {
            get => int.Parse(_properties[nameof(ItemId)], CultureInfo.InvariantCulture);

            set => _properties[nameof(ItemId)] = value.ToString(CultureInfo.InvariantCulture);
        }
        public int Version
        {
            get => int.Parse(_properties[nameof(Version)], CultureInfo.InvariantCulture);

            set => _properties[nameof(Version)] = value.ToString(CultureInfo.InvariantCulture);
        }
        public string Path
        {
            get => this[nameof(Path)];
            set => this[nameof(Path)] = value;
        }
        public int? ShareLinkCounter
        {
            get
            {
                var x = _properties[nameof(ShareLinkCounter)];
                if (x == null)
                {
                    return 0;
                }
                return int.Parse(x, CultureInfo.InvariantCulture);
            }

            set => _properties[nameof(ShareLinkCounter)] = value?.ToString(CultureInfo.InvariantCulture);
        }

        private string this[string key]
        {
            get
            {
                var result = _properties[key];
                if (result == null)
                    throw new InvalidOperationException();

                return result;
            }
            set
            {
                Check(key);
                _properties[key] = value;
            }
        }
        [Conditional("DEBUG")]
        private void Check(string key)
        {
            if (_properties[key] != null)
                throw new InvalidOperationException();
        }
    }

    public enum LinkType
    {
        None,
        ByPathAndDriveId,
        ByItemIdAndDriveId,
        ByVersionAndDriveId,
    }
}
