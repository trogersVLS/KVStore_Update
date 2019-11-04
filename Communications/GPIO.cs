using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MccDaq;
using ErrorDefs;

namespace KVStore_Update
{


    public class MccDaq_GPIO
    {
        MccDaq.MccBoard gpio_board;
        int numChannels;
        MccDaq.ErrorInfo err;
        MccDaq.DigitalPortType[] Ports = {DigitalPortType.FirstPortA, DigitalPortType.FirstPortB, DigitalPortType.FirstPortC,
                                          DigitalPortType.SecondPortA, DigitalPortType.SecondPortB, DigitalPortType.SecondPortCH, DigitalPortType.SecondPortCL,
                                          DigitalPortType.ThirdPortA, DigitalPortType.ThirdPortB, DigitalPortType.ThirdPortCH, DigitalPortType.ThirdPortCL };
        DigitalIO.clsDigitalIO dig_props = new DigitalIO.clsDigitalIO();

        public bool Connected = false;
        public MccDaq_GPIO()
        {
            InitUL();

            //TODO: add error handling for writes to ports when port is disables
            //dsetPort(DigitalPortType.FirstPortB, 10);

            //short x = getPort(DigitalPortType.FirstPortB);
        }

        public void setBit(DigitalPortType port, int bit, DigitalLogicState val)
        {
            this.gpio_board.DBitOut(port, bit, val);
        }

        public void setPort(DigitalPortType port, ushort val)
        {
            this.gpio_board.DOut(port, val);
        }

        public short getPort(DigitalPortType port)
        {
            short val;

            this.gpio_board.DIn(port, out val);

            return val;
        }

        public int getBit(DigitalPortType port, int bit)
        {
            return 5;
        }

        private void InitUL()
        {
            //  Initiate error handling
            //   activating error handling will trap errors like
            //   bad channel numbers and non-configured conditions.
            //   Parameters:
            //     MccDaq.ErrorReporting.PrintAll :all warnings and errors encountered will be printed
            //     MccDaq.ErrorHandling.StopAll   :if an error is encountered, the program will stop
            


            clsErrorDefs.ReportError = MccDaq.ErrorReporting.PrintAll;
            clsErrorDefs.HandleError = MccDaq.ErrorHandling.StopAll;
            this.gpio_board = new MccDaq.MccBoard();
            this.err = MccDaq.MccService.ErrHandling
                (ErrorReporting.PrintAll, ErrorHandling.StopAll);

            this.err = this.gpio_board.BoardConfig.GetDiNumDevs(out this.numChannels);



            if (this.numChannels != 0)
            {
                this.Connected = true;
                for (int i = 0; i < (numChannels - 1); i++)
                {
                    err = this.gpio_board.DConfigPort(Ports[i], DigitalPortDirection.DigitalOut);
                    this.setPort(Ports[i], 0);
                }
            }
            else
            {
                //GPIO is not configured
                this.Connected = false;
                this.gpio_board = null;
            }


            
        }
        public bool ConnectDevice()
        {
            InitUL();
            return this.Connected;
        }

    }


}
