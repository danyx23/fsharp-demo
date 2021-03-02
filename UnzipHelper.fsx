open System.IO

let unzipGzFile (gzFilename : string) (outputFilename : string) =
    use gzFileStream = File.OpenRead(gzFilename)
    use unzippedFileStream = File.Create(outputFilename)
    use decompressionStream = new System.IO.Compression.GZipStream(gzFileStream, System.IO.Compression.CompressionMode.Decompress)

    decompressionStream.CopyTo(unzippedFileStream)
    ()