using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib;
using ChessLib.Figures;
using SFML.Graphics;

namespace ChessSFML
{
    internal class SkinProvider
    {
        public Dictionary<(EnumFigure, ChessLib.Color), Sprite> Sprites = new Dictionary<(EnumFigure, ChessLib.Color), Sprite>();
        private Texture Textures;
        public SkinProvider(string fileName)
        {
            Textures = new Texture(fileName);
            for (int i = 0; i < 2; i++)
            {
                Sprites[(EnumFigure.Queen, i == 0 ? ChessLib.Color.Black : ChessLib.Color.White)] = new Sprite(Textures, new IntRect((int)(0 * Textures.Size.X / 6), (int)(i * Textures.Size.Y / 2), (int)(Textures.Size.X / 6), (int)(Textures.Size.Y / 2)));
                Sprites[(EnumFigure.King, i == 0 ? ChessLib.Color.Black : ChessLib.Color.White)] = new Sprite(Textures, new IntRect((int)(1 * Textures.Size.X / 6), (int)(i * Textures.Size.Y / 2), (int)(Textures.Size.X / 6), (int)(Textures.Size.Y / 2)));
                Sprites[(EnumFigure.Rook, i == 0 ? ChessLib.Color.Black : ChessLib.Color.White)] = new Sprite(Textures, new IntRect((int)(2 * Textures.Size.X / 6), (int)(i * Textures.Size.Y / 2), (int)(Textures.Size.X / 6), (int)(Textures.Size.Y / 2)));
                Sprites[(EnumFigure.Knight, i == 0 ? ChessLib.Color.Black : ChessLib.Color.White)] = new Sprite(Textures, new IntRect((int)(3 * Textures.Size.X / 6), (int)(i * Textures.Size.Y / 2), (int)(Textures.Size.X / 6), (int)(Textures.Size.Y / 2)));
                Sprites[(EnumFigure.Bishop, i == 0 ? ChessLib.Color.Black : ChessLib.Color.White)] = new Sprite(Textures, new IntRect((int)(4 * Textures.Size.X / 6), (int)(i * Textures.Size.Y / 2), (int)(Textures.Size.X / 6), (int)(Textures.Size.Y / 2)));
                Sprites[(EnumFigure.Pawn, i == 0 ? ChessLib.Color.Black : ChessLib.Color.White)] = new Sprite(Textures, new IntRect((int)(5 * Textures.Size.X / 6), (int)(i * Textures.Size.Y / 2), (int)(Textures.Size.X / 6), (int)(Textures.Size.Y / 2)));
            }

            foreach (var item in Sprites)
            {
                item.Value.Origin = new SFML.System.Vector2f(item.Value.TextureRect.Width / 2, item.Value.TextureRect.Height / 2); 
            }
        }
    }
}
