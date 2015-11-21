namespace OneCore.Hardware.Lifx.Core
{
    using System.Collections;

#if MF_FRAMEWORK_VERSION_V4_3
    using Microsoft.SPOT;
#else
    using System.Diagnostics;
#endif

    using OneCore.Hardware.Lifx.Contracts;
    using OneCore.Hardware.Lifx.Messages;

    public class LifxHub : ILifxHub, INetworkClient
    {
        #region Private      

        // spells out "LIFXV2" - version 2 of the app changes the site address to this...
        private readonly byte[] site_mac = { 0x4c, 0x49, 0x46, 0x58, 0x56, 0x32 };

#if MF_FRAMEWORK_VERSION_V4_3
        private readonly ArrayList bulbs = new ArrayList();
#else
        private readonly System.Collections.Generic.List<ILifxBulb> bulbs = new System.Collections.Generic.List<ILifxBulb>();
#endif

        #endregion

        #region Public

        #region Properties

        public byte[] Mac
        {
            get { return this.site_mac; }
        }

        public int BulbsCount
        {
            get { return this.bulbs.Count; }
        }

        #endregion

        #region Methods

        public ILifxBulb Bulbs(int index)
        {
            return (ILifxBulb)this.bulbs[index];
        }

        public void AddBulb(ILifxBulb bulb)
        {
            this.bulbs.Add(bulb);
        }

        public void RemoveBulb(ILifxBulb bulb)
        {
            this.bulbs.Remove(bulb);
        }

        public void MessageReceived(INetworkService service, IStream stream)
        {
            var headerRaw = new byte[LifxPacketInfo.Size];

            stream.ReadBytes(headerRaw);

            var packet = LifxPacket.Parse(headerRaw);

            if (packet.PayloadSize > 0)
            {
                stream.ReadBytes(packet.Payload);
            }

            this.Response(packet, stream);
        }

        #endregion

        #endregion

        #region Private

        private void Response(LifxPacket packet, IStream stream)
        {
            byte[] data;

            foreach (ILifxBulb bulb in this.bulbs)
            {
                switch (packet.Info.PacketType)
                {
                    case LifxPacketType.GetStateService:
                        {
                            var response = new LifxServiceStateResponse();

                            data = new byte[response.Size];

                            response.Info.BulbAddress = bulb.Address;

                            response.Service = LifxService.Udp;

                            response.ToByteArray(this.Mac, data);

                            stream.WriteBytes(data);
                            stream.Store();
                        }
                        break;

                    case LifxPacketType.GetWiFiFirmwareState:
                        {
                            var response = new LifxWiFiFirmwareResponse();

                            data = new byte[response.Size];

                            response.Info.BulbAddress = bulb.Address;

                            response.ToByteArray(this.Mac, data);

                            stream.WriteBytes(data);
                            stream.Store();
                        }
                        break;

                    case LifxPacketType.GetMeshFirmwareState:
                        {
                            var response = new LifxMeshFirmwareResponse();

                            data = new byte[response.Size];

                            response.Info.BulbAddress = bulb.Address;

                            response.ToByteArray(this.Mac, data);

                            stream.WriteBytes(data);
                            stream.Store();
                        }
                        break;

                    case LifxPacketType.GetVersion:
                        {
                            var response = new LifxVersionResponse();

                            data = new byte[response.Size];

                            response.Info.BulbAddress = bulb.Address;

                            response.ToByteArray(this.Mac, data);

                            stream.WriteBytes(data);
                            stream.Store();
                        }
                        break;

                    case LifxPacketType.GetLightState:
                        {
                            var response = new LifxLightStateResponse();

                            data = new byte[response.Size];

                            response.Info.BulbAddress = bulb.Address;

                            bulb.ToByteArray(response.Payload, 0);

                            response.ToByteArray(this.Mac, data);

                            stream.WriteBytes(data);
                            stream.Store();
                        }
                        break;

                    case LifxPacketType.GetPowerState:
                        {
                            var response = new LifxPowerStateResponse(bulb);
  
                            data = new byte[response.Size];
                            response.ToByteArray(this.Mac, data);

                            stream.WriteBytes(data);
                            stream.Store();
                        }
                        break;

                    case LifxPacketType.SetPowerState:
                        {
                            if (bulb.Address == packet.Info.BulbAddress)
                            {
                                var request = new LifxPowerStateRequest(packet);
                                bulb.Power =  request.Power;

                                //var response = new LifxPowerStateResponse(bulb);

                                //data = new byte[response.Size];
                                //response.ToByteArray(this.Mac, data);

                                //stream.WriteBytes(data);
                                //stream.Store().Wait();

                                return;                            
                            }
                        }
                        break;

                    case LifxPacketType.SetLightState:
                        {
                            if (bulb.Address == packet.Info.BulbAddress)
                            {
                                var request = new LifxLightStateRequest(packet);
                                bulb.Color = request.Color;
                                return;
                            }
                        }
                        break;

                    case LifxPacketType.GetLocation:
                        {
                            if (bulb.Address == packet.Info.BulbAddress)
                            {
                                var response = new LifxLocationResponse(bulb);

                                data = new byte[response.Size];
                                response.ToByteArray(this.Mac, data);

                                stream.WriteBytes(data);
                                stream.Store();

                                return;
                            }
                        }
                        break;

                    default:
                        {
                            //Debug.WriteLine(">> Not processed: {0}", packet.Info.PacketType);
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
