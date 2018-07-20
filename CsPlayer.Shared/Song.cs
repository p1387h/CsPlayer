using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsPlayer.Shared
{
    public class Song : ISong
    {
        public string Name { get; private set; }
        public string FilePath { get; private set; }
        public bool Valid { get; set; }

        public Song(string filePath, bool valid = true)
        {
            FilePath = filePath;
            Valid = valid;
            Name = Path.GetFileName(filePath).Split('.').First();
        }

        public bool Verify()
        {
            var exists = File.Exists(FilePath);
            Valid = exists;

            return exists;
        }
    }
}
