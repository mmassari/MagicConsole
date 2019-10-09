using System;
using System.Collections.Generic;
using System.Text;

namespace MagicConsole
{
    /// <summary>
    /// Rappresenta un messaggio da inviare alla console
    /// </summary>
    public class ConsoleMessage
    {

        #region Enums

        public enum MessageTypes
        {
            None,
            Info,
            Warning,
            Error
        }

        #endregion

        #region Declarations

        string _message;    //Messaggio da inviare
        ConsoleColor _foreColor;    //Colore carattere
        ConsoleColor _backColor;   //Colore di sfondo
        bool _addDate;  //Visualizzo la data all'inizio del messaggio?
        MessageTypes _messageType;  //Tipo di messaggio INFO, WARNING, ERROR

        #endregion

        #region Properties

        /// <summary>
        /// Messaggio da inviare
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// Colore del carattere
        /// </summary>
        public ConsoleColor ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }

        /// <summary>
        /// Colore di sfondo
        /// </summary>
        public ConsoleColor BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        /// <summary>
        /// Aggiungere la data all'inizio del messaggio?
        /// </summary>
        public bool AddDate
        {
            get { return _addDate; }
            set { _addDate = value; }
        }

        /// <summary>
        /// Tipo di messaggio
        /// </summary>
        public MessageTypes MessageType
        {
            get { return _messageType; }
            set { _messageType = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Costruttore che inizializza solo il messaggio
        /// Il ForeColor è grigio ed il BackColor è nero
        /// </summary>
        /// <param name="msg">Messaggio da visualizzare</param>
        public ConsoleMessage(string msg)
            : this(msg, ConsoleColor.Gray, ConsoleColor.Black, false, MessageTypes.None) { }

        /// <summary>
        /// Costruttore che inizializza il messaggio ed
        /// il foreColor, il backcolor è nero
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="forecolor"></param>
        public ConsoleMessage(string msg, ConsoleColor forecolor)
            : this(msg, forecolor, ConsoleColor.Black, false, MessageTypes.None) { }

        /// <summary>
        /// Costruttore che inizializza il messaggio 
        /// ed i colori in primo piano e di sfondo
        /// Non viene visualizzato il tipo e la data
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        public ConsoleMessage(string msg, ConsoleColor forecolor, ConsoleColor backcolor)
            : this(msg, forecolor, backcolor, false, MessageTypes.None) { }

        /// <summary>
        /// Costruttore che inizializza il messaggio 
        /// il colore in primo piano, tipo di messaggio e data
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        public ConsoleMessage(string msg, ConsoleColor forecolor,
            bool addDate, MessageTypes msgType)
            : this(msg, forecolor, ConsoleColor.Black, addDate, msgType) { }

        public ConsoleMessage(string msg, bool addDate, MessageTypes msgType)
            : this(msg, ConsoleColor.Gray, ConsoleColor.Black, addDate, msgType) { }

        /// <summary>
        /// Costruttore principale dove si inizializzano
        /// tutte le proprietà della class
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        public ConsoleMessage(string msg, ConsoleColor forecolor, ConsoleColor backcolor,
            bool addDate, MessageTypes msgType)
        {
            _message = msg;
            _foreColor = forecolor;
            _backColor = backcolor;
            _addDate = addDate;
            _messageType = msgType;
        }

        #endregion

        #region Methods

        public string GetFormattedMessage()
        {
            string msg = string.Empty;
            DateTime dt = DateTime.Now;

            if (_addDate)
                msg += dt.ToLongTimeString() + "." + dt.Millisecond.ToString().PadLeft(3, '0') + " ";

            switch (_messageType)
            {
                case MessageTypes.None:
                    msg += "     ";
                    break;
                case MessageTypes.Info:
                    msg += "INFO ";
                    break;
                case MessageTypes.Warning:
                    msg += "WARN ";
                    break;
                case MessageTypes.Error:
                    msg += "ERR  ";
                    break;
            }

            msg += _message;
            return msg;
        }

        #endregion

    }
}
