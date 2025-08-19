using System;
using System.Drawing;

namespace BaiTapCuoiKy
{
    /// <summary>
    /// Enum ?? xác ??nh lo?i tin nh?n ???c g?i qua m?ng.
    /// </summary>
    [Serializable]
    public enum Command
    {
        Login,          // G?i thông tin khi m?t ng??i ch?i m?i k?t n?i
        PlayerList,     // G?i danh sách t?t c? ng??i ch?i hi?n t?i
        Message,        // G?i m?t tin nh?n chat
        Draw,           // G?i d? li?u nét v?
        ClearCanvas,    // Yêu c?u xóa b?ng v?
        StartGame,      // B?t ??u game
        StartRound,     // B?t ??u m?t vòng m?i
        WordToGuess,    // G?i t? c?n ?oán (cho ng??i v?) ho?c các d?u g?ch (cho ng??i ?oán)
        Guess,          // G?i m?t l??t ?oán t?
        CorrectGuess,   // Thông báo có ng??i ?oán ?úng
        UpdateScore,    // C?p nh?t ?i?m s?
        TimeUpdate,     // C?p nh?t th?i gian còn l?i
        EndOfRound,     // Thông báo k?t thúc vòng
        EndOfGame,      // Thông báo k?t thúc game
        Disconnect      // Thông báo m?t ng??i ch?i ?ã ng?t k?t n?i
    }

    /// <summary>
    /// L?p c? s? ?? ?óng gói d? li?u g?i ?i. M?i d? li?u g?i qua m?ng s? k? th?a t? l?p này.
    /// </summary>
    [Serializable]
    public class NetworkData
    {
        public Command Command { get; set; }
        public string SenderName { get; set; }
        public object Payload { get; set; } // D? li?u chính, có th? là b?t k? ki?u gì

        public NetworkData(Command command, string senderName, object payload = null)
        {
            Command = command;
            SenderName = senderName;
            Payload = payload;
        }

        // Chuy?n ??i ??i t??ng NetworkData thành m?t m?ng byte ?? g?i ?i
        public byte[] ToBytes()
        {
            using (var ms = new System.IO.MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        // Chuy?n ??i m?t m?ng byte nh?n ???c thành ??i t??ng NetworkData
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
    /// D? li?u cho nét v?.
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
