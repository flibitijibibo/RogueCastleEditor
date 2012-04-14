using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class GridObj
    {
        private int NUM_SQUARES = 300;

        private int m_pixelWidth;
        private int m_pixelHeight;
        private Texture2D m_gridTexture;
        private List<Rectangle> m_rowLines;
        private List<Rectangle> m_colLines;

        private int m_maxWidth;
        private int m_maxHeight;
        private int m_currentWidth;
        private int m_currentHeight;

        private int m_rowToShift; // The amount the grid should shift by to give it the illusion of moving.
        private int m_colToShift;

        public GridObj(GraphicsDevice graphicsDevice)
        {
            m_maxWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            m_maxHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            m_currentWidth = graphicsDevice.Viewport.Width;
            m_currentHeight = graphicsDevice.Viewport.Height;
      
            m_gridTexture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Int32[] pixel = { 0xFFFFFF }; // White. 0xFF is Red, 0xFF0000 is Blue
            m_gridTexture.SetData<Int32>(pixel, 0, 1);
        }

        public void CreateGrid(int pixelWidth, int pixelHeight)
        {
            m_pixelWidth = pixelWidth;
            m_pixelHeight = pixelHeight;

            if (m_rowLines != null)
                m_rowLines.Clear();
            else
                m_rowLines = new List<Rectangle>();

            if (m_colLines != null)
                m_colLines.Clear();
            else
                m_colLines = new List<Rectangle>();

            //int yOffset = (int)Math.Round(((m_currentWidth/2f) % m_pixelWidth), MidpointRounding.AwayFromZero);
            int yOffset = (int)((Math.Round(m_currentWidth / 2f / m_pixelWidth, MidpointRounding.AwayFromZero)));
            yOffset = (int)(yOffset * m_pixelWidth - m_currentWidth / 2f);

            //int xOffset = (int)Math.Round(((m_currentHeight / 2f) % m_pixelHeight), MidpointRounding.AwayFromZero);
            int xOffset = (int)((Math.Round(m_currentHeight / 2f / m_pixelHeight, MidpointRounding.AwayFromZero)));
            xOffset = (int)(xOffset * m_pixelHeight - m_currentHeight / 2f);

            for (int i = -NUM_SQUARES/2; i < NUM_SQUARES/2; i++)
            {
                m_colLines.Add(new Rectangle(i * m_pixelWidth - yOffset, 0, 1, m_maxHeight));
            }

            for (int j = -NUM_SQUARES/2; j < NUM_SQUARES/2; j++)
            {
                m_rowLines.Add(new Rectangle(0, j * m_pixelHeight - xOffset, m_maxWidth, 1));
            }

        }

        public void RecalculatePosition(Camera2D camera)
        {
            m_colToShift = (int)(camera.X / ColDistance);
            m_colToShift *= ColDistance;
            m_colToShift = (int)(camera.X - m_colToShift);

            m_rowToShift = (int)(camera.Y / RowDistance);
            m_rowToShift *= RowDistance;
            m_rowToShift = (int)(camera.Y - m_rowToShift);
        }

        public void Draw(Camera2D camera)
        {
            //if (camera.CenteredZoom)
            {
                foreach (Rectangle rect in m_colLines)
                {
                    Rectangle newRect = new Rectangle((int)((rect.X - m_colToShift - camera.Width/2) * camera.Zoom),
                                                        -camera.Width/2, rect.Width, rect.Height);
                    camera.Draw(m_gridTexture, newRect, Consts.GRID_COLOR);
                }

                foreach (Rectangle rect in m_rowLines)
                {
                    Rectangle newRect = new Rectangle(-camera.Height,
                                                        (int)((rect.Y - m_rowToShift - camera.Height/2) * camera.Zoom),
                                                        rect.Width, rect.Height);
                    camera.Draw(m_gridTexture, newRect, Consts.GRID_COLOR);
                }
            }
        }

        public int RowDistance
        {
            get { return m_pixelHeight; }
        }

        public int ColDistance
        {
            get { return m_pixelWidth; }
        }

        public int PixelsShiftedX
        {
            get { return m_colToShift; }
        }

        public int PixelsShiftedY
        {
            get { return m_rowToShift; }
        }
    }
}
