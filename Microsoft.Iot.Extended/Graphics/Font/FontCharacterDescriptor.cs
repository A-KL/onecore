namespace Microsoft.Iot.Extended.Graphics.Font
{
    /* FontCharacterDescripter contains font information for a  single character */
    public class FontCharacterDescriptor
    {
        public readonly char Character;
        public readonly uint CharacterWidthPx;
        public readonly uint CharacterHeightBytes;
        public readonly byte[] CharacterData;

        public FontCharacterDescriptor(char chr, uint charHeightBytes, byte[] charData)
        {
            this.Character = chr;
            this.CharacterWidthPx = (uint)charData.Length / charHeightBytes;
            this.CharacterHeightBytes = charHeightBytes;
            this.CharacterData = charData;
        }
    }
}
