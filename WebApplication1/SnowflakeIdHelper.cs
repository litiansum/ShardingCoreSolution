using IdGen;
using System.Net.NetworkInformation;

namespace WebApplication1
{
    public static class SnowflakeIdHelper
    {
        /// <summary>
        /// Start time 2023-01-01 00:00:00
        /// </summary>
        public static readonly DateTime Epoch = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The number of bits occupied by workerId
        /// </summary>
        private const int WorkerIdBits = 10;

        /// <summary>
        /// The number of bits occupied by timestamp
        /// </summary>
        private const int TimestampBits = 41;

        /// <summary>
        /// The number of bits occupied by sequence
        /// </summary>
        private const int SequenceBits = 12;

        private static readonly object SLock = new();

        private static IdGenerator? _idGenerator;

        /// <summary>
        /// Maximum supported machine id, the result is 1023
        /// </summary>
        private const int MaxWorkerId = ~(-1 << WorkerIdBits);

        public static IdGenerator Default()
        {
            if (_idGenerator != null) return _idGenerator;

            lock (SLock)
            {
                if (_idGenerator != null) return _idGenerator;

                if (!int.TryParse(Environment.GetEnvironmentVariable("SINOVISION_WORKERID"), out var workerId))
                    workerId = Util.GenerateWorkerId(MaxWorkerId);

                // Create an ID with 45 bits for timestamp, 2 for generator-id 
                // and 16 for sequence
                var structure = new IdStructure(TimestampBits, WorkerIdBits, SequenceBits);
                // Prepare options
                var options = new IdGeneratorOptions(structure, new DefaultTimeSource(Epoch));

                return _idGenerator = new IdGenerator(workerId, options);
            }
        }



        public static DateTimeOffset GetDateTime(long snowflakeId)
        {
            _idGenerator ??= Default();

            var id = _idGenerator.FromId(snowflakeId);

            return id.DateTimeOffset;
        }
    }

    internal static class Util
    {
        /// <summary>
        /// auto generate workerId, try using mac first, if failed, then randomly generate one
        /// </summary>
        /// <returns>workerId</returns>
        public static int GenerateWorkerId(int maxWorkerId)
        {
            try
            {
                return GenerateWorkerIdBaseOnMac();
            }
            catch
            {
                return GenerateRandomWorkerId(maxWorkerId);
            }
        }

        /// <summary>
        /// use lowest 10 bit of available MAC as workerId
        /// </summary>
        /// <returns>workerId</returns>
        private static int GenerateWorkerIdBaseOnMac()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            //exclude virtual and Loopback
            var firstUpInterface = nics.OrderByDescending(x => x.Speed).FirstOrDefault(x =>
                !x.Description.Contains("Virtual") && x.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                x.OperationalStatus == OperationalStatus.Up);
            if (firstUpInterface == null) throw new NotSupportedException("no available mac found");
            var address = firstUpInterface.GetPhysicalAddress();
            var mac = address.GetAddressBytes();

            return ((mac[4] & 0B11) << 8) | (mac[5] & 0xFF);
        }

        /// <summary>
        /// randomly generate one as workerId
        /// </summary>
        /// <returns></returns>
        private static int GenerateRandomWorkerId(int maxWorkerId)
        {
            return new Random().Next(maxWorkerId + 1);
        }
    }
}
