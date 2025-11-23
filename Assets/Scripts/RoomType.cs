using UnityEngine;

public enum RoomType
{
    Empty,    // Ruangan kosong, jarang digunakan di level 1
    Combat,   // Pertarungan/Fight Scene
    Boss,     // Ruangan Boss
    Entrance, // Ruangan Awal Player
    Exit      // Pintu Keluar (Tidak diperlukan jika hanya 1 Boss room)
}
