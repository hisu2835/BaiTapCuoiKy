using System;
using System.Drawing;

namespace BaiTapCuoiKy
{
    /// <summary>
    /// Enum ?? x�c ??nh lo?i tin nh?n ???c g?i qua m?ng.
    /// </summary>
    [Serializable]
    public enum Command
    {
        Login,          // G?i th�ng tin khi m?t ng??i ch?i m?i k?t n?i
        PlayerList,     // G?i danh s�ch t?t c? ng??i ch?i hi?n t?i
        Message,        // G?i m?t tin nh?n chat
        Draw,           // G?i d? li?u n�t v?
        ClearCanvas,    // Y�u c?u x�a b?ng v?
        StartGame,      // B?t ??u game
        StartRound,     // B?t ??u m?t v�ng m?i
        WordToGuess,    // G?i t? c?n ?o�n (cho ng??i v?) ho?c c�c d?u g?ch (cho ng??i ?o�n)
        Guess,          // G?i m?t l??t ?o�n t?
        CorrectGuess,   // Th�ng b�o c� ng??i ?o�n ?�ng
        UpdateScore,    // C?p nh?t ?i?m s?
        TimeUpdate,     // C?p nh?t th?i gian c�n l?i
        EndOfRound,     // Th�ng b�o k?t th�c v�ng
        EndOfGame,      // Th�ng b�o k?t th�c game
        Disconnect      // Th�ng b�o m?t ng??i ch?i ?� ng?t k?t n?i
    }

    /// <summary>
    /// L?p c? s? ?? ?�ng g�i d? li?u g?i ?i. M?i d? li?u g?i qua m?ng s? k? th?a t? l?p n�y.
    /// </summary>
    [Serializable]
    public class NetworkData
    {
        public Command Command { get; set; }
        public string SenderName { get; set; }
        public object Payload { get; set; } // D? li?u ch�nh, c� th? l� b?t k? ki?u g�

        public NetworkData(Command command, string senderName, object payload = null)
        {
            Command = command;
            SenderName = senderName;
            Payload = payload;
        }

        // Chuy?n ??i ??i t??ng NetworkData th�nh m?t m?ng byte ?? g?i ?i
        public byte[] ToBytes()
        {
            using (var ms = new System.IO.MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        // Chuy?n ??i m?t m?ng byte nh?n ???c th�nh ??i t??ng NetworkData
        public static NetworkData FromBytes(byte[] bytes)
        {
            using (var ms = new System.IO.MemoryStream(bytes))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (NetworkData)formatter.Deserialize(ms);
            }
        }
    }

    /// <summary>
    /// D? li?u cho n�t v?.
    /// </summary>
    [Serializable]
    public class DrawPayload
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public Color PenColor { get; set; }
        public int PenWidth { get; set; }
        public bool IsEraser { get; set; }
    }
}
