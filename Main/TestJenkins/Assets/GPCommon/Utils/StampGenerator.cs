namespace GPCommon
{
    public class StampGenerator
    {

        private byte index;
        private byte max;

        public StampGenerator(byte p_max = byte.MaxValue)
        {
            index = byte.MinValue;
            max = p_max;
        }

        public byte GetStamp()
        {
            if (index >= max)
            {
                index = byte.MinValue;
                return index;
            }
            else
            {
                return ++index;
            }
        }
    }
}
