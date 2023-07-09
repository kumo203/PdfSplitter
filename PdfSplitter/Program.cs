// NuGet:
// itext7
// itext7.bouncy-castle-adapter
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System.Reflection.PortableExecutable;

namespace PDFSplitter
{
    public class PDFSplitter
    {
        static public string targetPdfPath { get; set; }
        static void Main(string[] args)
        {
            if (args.Length <= 0 || !File.Exists(args[0]))
            {
                System.Console.WriteLine("Usage: PDFSplitter PDF_file {page_count}");
                return;
            }
            targetPdfPath = args[0];

            int maxPageCount = 1; // create a new PDF per X pages from the original file
            if (args.Length >= 2)
            {
                maxPageCount = int.Parse(args[1]);
            }
            PdfReader reader = new PdfReader(targetPdfPath);
            reader.SetUnethicalReading(true);
            PdfDocument pdfDocument = new PdfDocument(reader);
            IList<PdfDocument> splitDocuments = new CustomPdfSplitter(pdfDocument, targetPdfPath).SplitByPageCount(maxPageCount);

            foreach (PdfDocument doc in splitDocuments)
            {
                doc.Close();
            }

            pdfDocument.Close();

            System.Console.WriteLine("Done.");
        }
    }

    class CustomPdfSplitter : PdfSplitter
    {
        int _partNumber = 1;
        public string targetPdfPath { get; set; }

        public CustomPdfSplitter(PdfDocument pdfDocument, string targetPdfPath) : base(pdfDocument)
        {
            this.targetPdfPath = targetPdfPath;
        }

        protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
        {
            try
            {
                return new PdfWriter($"{Path.GetFileNameWithoutExtension(targetPdfPath)}_{_partNumber++:000}.pdf");
            }
            catch (FileNotFoundException e)
            {
                throw new SystemException(e.Message, e);
            }
        }
    }
}
