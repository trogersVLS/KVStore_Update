using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MccDaq;

namespace KVStore_Update
{

    public struct GPIO_PIN
    {
        public DigitalPortType port;
        public short val;

        public GPIO_PIN(DigitalPortType port, short val)
        {   //Defines the port name and port value of the associated GPIO signal
            //In order to do "atomic" writes to the GPIO module using multiple signals, 
            //they will need to share the same port and be OR'd together
            this.port = port;
            this.val = val;
        }

    }

    static class GPIO_Definitions
    {
        public static readonly GPIO_PIN INT_BATT_EN;
        public static readonly  GPIO_PIN EXT1_BATT_EN;
        public static readonly  GPIO_PIN EXT2_BATT_EN;
        public static readonly GPIO_PIN PPS_LOAD_EN;

        public static readonly GPIO_PIN MEAS_3V3_HOT_EN;
        public static readonly GPIO_PIN MEAS_5V0_HOT_EN;
        public static readonly GPIO_PIN MEAS_5V3_EN;
        public static readonly GPIO_PIN MEAS_12V0_EN;
        public static readonly GPIO_PIN MEAS_3V3_EN;
        public static readonly GPIO_PIN MEAS_1V2_EN;
        public static readonly GPIO_PIN MEAS_3V3A_EN;
        public static readonly GPIO_PIN MEAS_VREF_EN;
        public static readonly GPIO_PIN MEAS_30V_EN;
        public static readonly GPIO_PIN MEAS_36V_EN;

        public static readonly GPIO_PIN MEAS_O2_SV1N_EN;
        public static readonly GPIO_PIN MEAS_O2_SV2N_EN;

        public static readonly GPIO_PIN FAN_FREQ_MEAS_EN;
        public static readonly GPIO_PIN VFAN_MEAS_EN;
        public static readonly GPIO_PIN FAN_FAULT_EN;

        public static readonly GPIO_PIN EXT_O2_DIS;

        public static readonly GPIO_PIN WDOG_DIS;

        public static readonly GPIO_PIN MEAS_BATT_CHG_EN;

        public static readonly GPIO_PIN AC_EN;

        static GPIO_Definitions()
        {   //FirstPortA -> 8 bits wide
            INT_BATT_EN =       new GPIO_PIN(DigitalPortType.FirstPortA, 0x03); //Bits 0&1
            EXT1_BATT_EN =      new GPIO_PIN(DigitalPortType.FirstPortA, 0x0C); //Bits 2&3
            EXT2_BATT_EN =      new GPIO_PIN(DigitalPortType.FirstPortA, 0x30); //Bits 4&5

            PPS_LOAD_EN =       new GPIO_PIN(DigitalPortType.FirstPortA, 0x80); //Bit 7

            //FirstPortB -> 8 bits wide
            MEAS_3V3_HOT_EN =   new GPIO_PIN(DigitalPortType.FirstPortB, 0x01); //Bit 0
            MEAS_5V0_HOT_EN =   new GPIO_PIN(DigitalPortType.FirstPortB, 0x02); //Bit 1
            MEAS_5V3_EN =       new GPIO_PIN(DigitalPortType.FirstPortB, 0x04); //Bit 2
            MEAS_12V0_EN =      new GPIO_PIN(DigitalPortType.FirstPortB, 0x08); //Bit 3
            MEAS_3V3_EN =       new GPIO_PIN(DigitalPortType.FirstPortB, 0x10); //Bit 4
            MEAS_1V2_EN =       new GPIO_PIN(DigitalPortType.FirstPortB, 0x20); //Bit 5
            MEAS_3V3A_EN =      new GPIO_PIN(DigitalPortType.FirstPortB, 0x40); //Bit 6
            MEAS_VREF_EN =      new GPIO_PIN(DigitalPortType.FirstPortB, 0x80); //Bit 7

            //FirstPortC -> 8 bits wide
            MEAS_30V_EN =       new GPIO_PIN(DigitalPortType.FirstPortC, 0x01); //Bit 0
            MEAS_36V_EN =       new GPIO_PIN(DigitalPortType.FirstPortC, 0x02); //Bit 1
            MEAS_O2_SV1N_EN =   new GPIO_PIN(DigitalPortType.FirstPortC, 0x04); //Bit 2
            MEAS_O2_SV2N_EN =   new GPIO_PIN(DigitalPortType.FirstPortC, 0x08); //Bit 3
            FAN_FREQ_MEAS_EN =  new GPIO_PIN(DigitalPortType.FirstPortC, 0x10); //Bit 4
            VFAN_MEAS_EN =      new GPIO_PIN(DigitalPortType.FirstPortC, 0x20); //Bit 5
            MEAS_BATT_CHG_EN =  new GPIO_PIN(DigitalPortType.FirstPortC, 0x40); //Bit 6

            //SecondPortA -> 8 bits wide
            FAN_FAULT_EN =      new GPIO_PIN(DigitalPortType.SecondPortA, 0x01); //Bit 0
            EXT_O2_DIS =        new GPIO_PIN(DigitalPortType.SecondPortA, 0x06); //Bits 1&2
            WDOG_DIS =          new GPIO_PIN(DigitalPortType.SecondPortA, 0x08); //Bit 3

            //SecondPortC -> 4 bits wide
            AC_EN =             new GPIO_PIN(DigitalPortType.SecondPortCL, 0x1); // Bit 1

        }


    }

}
