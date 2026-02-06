namespace Utilities.Static
{
    public class PdfTableColumn
    {
        public string? Label { get; set; }
        public string? PropertyName { get; set; }
        public float RelativeWidth { get; set; } = 1f;
        public PdfColumnAlignment Alignment { get; set; } = PdfColumnAlignment.Left;
        public bool IsBold { get; set; } = false;
    }

    public enum PdfColumnAlignment
    {
        Left,
        Center,
        Right
    }
}
