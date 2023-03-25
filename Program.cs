// CSIPatch: patch panorama errors in the Windows version of "CSI: Crime Scene Investigation" (2003)
// By Mikkel Krautz <mikkel@krautz.dk>, made available under CC0. No rights reserved.
//
// SPDX-License-Identifier: cc0-1.0

// Level 5 (called "level6" in the game files) in the "CSI: Crime Scene Investigation" is missing
// some of the panoramas used for some areas in the game when running the game using the software
// renderer. Luckily, the full panorama textures are available in the game install. Presumably,
// these are used by the Direct3D or OpenGL renderers.
//
// To make level 5 playable, this program creates panorama slices compatible with the games' software
// renderer.
//
// level6/panorama/motel6.jpg is sliced up into 256 pixels wide slices and placed in
// level6/panorama/motel6_SW with files named motel6N.jpg where N=1...8.
//
// level6/panorama/victim6.jpg is sliced up into 256 pixels wide slices and placed in
// level6/panorama/victim6_SW with files named motel6N.jpg where N=1...8.
//
// Finally, the leda_SW software panorama's first slice is misnamed. It is called leda.jpg
// in the original install, but needs to be renamed to leda1.jpg.
// This tool simply re-creates the slices from the original leda.jpg texture, as is the
// case for motel6 and victim6:
// level6/panorama/leda.jpg is sliced up into 256 pixels wide slices and placed in
// level6/panorama/leda_SW with files named ledaN.jpg where N=1...8.
//
// The files are output in the current working directory of the tool.
// After running the patcher, copy the leda_SW, motel6_SW, and victim6_SW folders
// into C:\Program Files\Ubisoft\369\CSI\level6\panorama. (Change path appropriately
// if the game is installed somewhere else.

using ImageMagick;

var programFiles = Environment.GetEnvironmentVariable("PROGRAMFILES");
if (programFiles is null)
{
    throw new Exception("no program files directory found");
}

var gameDir = Path.Combine(programFiles, "Ubisoft", "369", "CSI");
var level6PanoDir = Path.Combine(gameDir, "level6", "panorama");

var workDir = Directory.GetCurrentDirectory();

MakeSWPanorama("motel6_SW", Path.Combine(level6PanoDir, "motel6.jpg"), workDir);
MakeSWPanorama("victim6_SW", Path.Combine(level6PanoDir, "victim6.jpg"), workDir);
MakeSWPanorama("leda_SW", Path.Combine(level6PanoDir, "leda.jpg"), workDir);

Console.WriteLine($"Please copy motel6_SW, victim6_SW and leda_SW into {level6PanoDir}");

void MakeSWPanorama(string name, string baseImage, string outputDir)
{ 
    var imagePrefix = name.Replace("_SW", "");
    var panoDir = Path.Combine(outputDir, name);
    if (!Directory.Exists(panoDir))
    {
        Directory.CreateDirectory(panoDir);
        using (var fs = File.OpenRead(baseImage))
        {

            var img = new MagickImage(fs);
            for (var i = 0; i < 8; i++)
            {
                var subImg = img.Clone(i * 256, 0, 256, 512);
                subImg.Write(Path.Combine(panoDir, $"{imagePrefix}{i + 1}.jpg"));
            }
        }
    }

    Console.WriteLine($"Panorama {name} created");
}
