using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsPlayer.Shared
{
    public class Song
    {
        public string FilePath { get; set; }
        public bool Valid { get; set; }

        public Song(string filePath, bool valid = true)
        {
            FilePath = filePath;
            Valid = valid;
        }

        public bool Verify()
        {
            var exists = File.Exists(FilePath);
            Valid = exists;

            return exists;
        }
    }
}
