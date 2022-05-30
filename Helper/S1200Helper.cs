using System;

namespace OutReader.Helper
{
    public class S1200Helper
    {
        //public static libnodave.daveConnection dc;
        //public static libnodave.daveOSserialType fds;
        //public static libnodave.daveInterface di;
        //public static int res;
        //public static byte plcValue;
        //public static int memoryRes;
        //public static byte[] memoryBuffer = new byte[10];
        //public static object Get()
        //{
        //    int i, a = 0, j, res, b = 0, c = 0;
        //    float d = 0;
        //    fds.rfd = libnodave.openSocket(102, "192.168.25.1");
        //    fds.wfd = fds.rfd;
        //    di = new libnodave.daveInterface(fds, "IF1", 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed187k);
        //    //res = di.initAdapter();
        //    dc = new libnodave.daveConnection(di, 0, 0, 1);
        //    res = dc.connectPLC();
        //    if (0 == res)
        //    {
        //        res = dc.readBytes(libnodave.daveDB, 9, 0, 4, memoryBuffer);
        //        //                var zz = dc.getCounterValue();
        //        //res = dc.readBytes(libnodave.daveFlags, 0, 10, 16, memoryBuffer);
        //        res = dc.readBytes(libnodave.daveInputs, 0, 96, 2, memoryBuffer);
        //        res = dc.readBytes(libnodave.daveFlags, 68, 8, 4, memoryBuffer);
        //        //res = dc.readBytes(libnodave.daveFlags, 0, 0, 16, null);
        //        if (res == 0)
        //        {
        //            a = dc.getS32();
        //            b = dc.getS32();
        //            c = dc.getS32();
        //            d = dc.getFloat();
        //            Console.WriteLine("FD0: " + a);
        //            Console.WriteLine("FD4: " + b);
        //            Console.WriteLine("FD8: " + c);
        //            Console.WriteLine("FD12: " + d);
        //        }
        //        else
        //            Console.WriteLine("error " + res + ":  " + libnodave.daveStrerror(res));
        //    }
        //    //memoryRes = dc.readBytes(libnodave.daveDB, 10, 0, 1, null);
        //    //plcValue = memoryBuffer[0];
        //    dc.disconnectPLC();
        //    di.disconnectAdapter();
        //    libnodave.closePort(fds.rfd);
        //    Console.ReadLine();
        //    return null;
        //}
    }
}
