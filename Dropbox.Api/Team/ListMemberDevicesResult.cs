// <auto-generated>
// Auto-generated by StoneAPI, do not modify.
// </auto-generated>

namespace Dropbox.Api.Team
{
    using sys = System;
    using col = System.Collections.Generic;
    using re = System.Text.RegularExpressions;

    using enc = Dropbox.Api.Stone;

    /// <summary>
    /// <para>The list member devices result object</para>
    /// </summary>
    public class ListMemberDevicesResult
    {
        #pragma warning disable 108

        /// <summary>
        /// <para>The encoder instance.</para>
        /// </summary>
        internal static enc.StructEncoder<ListMemberDevicesResult> Encoder = new ListMemberDevicesResultEncoder();

        /// <summary>
        /// <para>The decoder instance.</para>
        /// </summary>
        internal static enc.StructDecoder<ListMemberDevicesResult> Decoder = new ListMemberDevicesResultDecoder();

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ListMemberDevicesResult" />
        /// class.</para>
        /// </summary>
        /// <param name="activeWebSessions">List of web sessions made by this team
        /// member</param>
        /// <param name="desktopClientSessions">List of desktop clients used by this team
        /// member</param>
        /// <param name="mobileClientSessions">List of mobile client used by this team
        /// member</param>
        public ListMemberDevicesResult(col.IEnumerable<ActiveWebSession> activeWebSessions = null,
                                       col.IEnumerable<DesktopClientSession> desktopClientSessions = null,
                                       col.IEnumerable<MobileClientSession> mobileClientSessions = null)
        {
            var activeWebSessionsList = enc.Util.ToList(activeWebSessions);

            var desktopClientSessionsList = enc.Util.ToList(desktopClientSessions);

            var mobileClientSessionsList = enc.Util.ToList(mobileClientSessions);

            this.ActiveWebSessions = activeWebSessionsList;
            this.DesktopClientSessions = desktopClientSessionsList;
            this.MobileClientSessions = mobileClientSessionsList;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ListMemberDevicesResult" />
        /// class.</para>
        /// </summary>
        /// <remarks>This is to construct an instance of the object when
        /// deserializing.</remarks>
        [sys.ComponentModel.EditorBrowsable(sys.ComponentModel.EditorBrowsableState.Never)]
        public ListMemberDevicesResult()
        {
        }

        /// <summary>
        /// <para>List of web sessions made by this team member</para>
        /// </summary>
        public col.IList<ActiveWebSession> ActiveWebSessions { get; protected set; }

        /// <summary>
        /// <para>List of desktop clients used by this team member</para>
        /// </summary>
        public col.IList<DesktopClientSession> DesktopClientSessions { get; protected set; }

        /// <summary>
        /// <para>List of mobile client used by this team member</para>
        /// </summary>
        public col.IList<MobileClientSession> MobileClientSessions { get; protected set; }

        #region Encoder class

        /// <summary>
        /// <para>Encoder for  <see cref="ListMemberDevicesResult" />.</para>
        /// </summary>
        private class ListMemberDevicesResultEncoder : enc.StructEncoder<ListMemberDevicesResult>
        {
            /// <summary>
            /// <para>Encode fields of given value.</para>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="writer">The writer.</param>
            public override void EncodeFields(ListMemberDevicesResult value, enc.IJsonWriter writer)
            {
                if (value.ActiveWebSessions.Count > 0)
                {
                    WriteListProperty("active_web_sessions", value.ActiveWebSessions, writer, Dropbox.Api.Team.ActiveWebSession.Encoder);
                }
                if (value.DesktopClientSessions.Count > 0)
                {
                    WriteListProperty("desktop_client_sessions", value.DesktopClientSessions, writer, Dropbox.Api.Team.DesktopClientSession.Encoder);
                }
                if (value.MobileClientSessions.Count > 0)
                {
                    WriteListProperty("mobile_client_sessions", value.MobileClientSessions, writer, Dropbox.Api.Team.MobileClientSession.Encoder);
                }
            }
        }

        #endregion


        #region Decoder class

        /// <summary>
        /// <para>Decoder for  <see cref="ListMemberDevicesResult" />.</para>
        /// </summary>
        private class ListMemberDevicesResultDecoder : enc.StructDecoder<ListMemberDevicesResult>
        {
            /// <summary>
            /// <para>Create a new instance of type <see cref="ListMemberDevicesResult"
            /// />.</para>
            /// </summary>
            /// <returns>The struct instance.</returns>
            protected override ListMemberDevicesResult Create()
            {
                return new ListMemberDevicesResult();
            }

            /// <summary>
            /// <para>Set given field.</para>
            /// </summary>
            /// <param name="value">The field value.</param>
            /// <param name="fieldName">The field name.</param>
            /// <param name="reader">The json reader.</param>
            protected override void SetField(ListMemberDevicesResult value, string fieldName, enc.IJsonReader reader)
            {
                switch (fieldName)
                {
                    case "active_web_sessions":
                        value.ActiveWebSessions = ReadList<ActiveWebSession>(reader, Dropbox.Api.Team.ActiveWebSession.Decoder);
                        break;
                    case "desktop_client_sessions":
                        value.DesktopClientSessions = ReadList<DesktopClientSession>(reader, Dropbox.Api.Team.DesktopClientSession.Decoder);
                        break;
                    case "mobile_client_sessions":
                        value.MobileClientSessions = ReadList<MobileClientSession>(reader, Dropbox.Api.Team.MobileClientSession.Decoder);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        #endregion
    }
}
