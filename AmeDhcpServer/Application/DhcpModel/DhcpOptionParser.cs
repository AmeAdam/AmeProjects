namespace AmeDhcpServer.Core
{
    public class DhcpOptionParser
    {
        public Dictionary<DhcpMessage.DhcpOptionCode, byte[]> GetOptions(byte[] options)
        {
            var result = new Dictionary<DhcpMessage.DhcpOptionCode, byte[]>();
           // options = TrimEmptyData(options);
            result = ParseOptions(options);
            
            return result;
        }

        private byte[] TrimEmptyData(byte[] options)
        {
            var endByteIndex = Array.IndexOf(options, (byte)DhcpMessage.DhcpOptionCode.End);
            byte[] trimmedArray = new byte[endByteIndex+1];

            Array.Copy(options, 0, trimmedArray, 0, endByteIndex+1);

            return trimmedArray;
        }

        private Dictionary<DhcpMessage.DhcpOptionCode, byte[]> ParseOptions(byte[] options)
        {
            int index = 0;
            int dataLength = options.Length;
            var result = new Dictionary<DhcpMessage.DhcpOptionCode, byte[]>();

            while (index < dataLength)
            {
                var opCode = (DhcpMessage.DhcpOptionCode)options[index];

                if (opCode == DhcpMessage.DhcpOptionCode.End) break;

                var length = options[++index];
                
                byte[] data = new byte[length];
                Array.Copy(options, ++index, data, 0, length);

                index += length;

                result.Add(opCode, data);
            }

            return result;
        }
    }
}
