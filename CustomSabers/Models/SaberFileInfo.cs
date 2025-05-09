using System;
using System.IO;

namespace CustomSabersLite.Models;

internal record SaberFileInfo(
    FileInfo FileInfo,
    string Hash,
    DateTime DateAdded);
