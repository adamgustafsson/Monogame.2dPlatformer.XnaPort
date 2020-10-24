using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace View
{
    /// <summary>
    /// Main class for rendering smoke effects
    /// </summary>
    class SmokeSystem
    {
        //Variables
        private const int PARTICLES_PER_ARRAY = 100;
        private List<SmokeParticle> m_particleList;
        private Texture2D m_smokeTexture;
        internal bool SmokeWasReset { get; set; }

        //Constructor
        public SmokeSystem()
        {
            LoadNewSmoke();
        }

        //Initsializing new smokeparticles
        internal void LoadNewSmoke()
        {
            this.m_particleList = new List<SmokeParticle>(PARTICLES_PER_ARRAY);
            int i = 0;
            while (i < PARTICLES_PER_ARRAY)
            {
                this.m_particleList.Add(new SmokeParticle(i));
                i++;
            }
        }

        //Load content
        internal void LoadContent(ContentManager a_content)
        {
            m_smokeTexture = a_content.Load<Texture2D>("Textures/particlesmoke");
        }

        //Draw method
        internal void UpdateAndDraw(float a_elapsedTime, Vector2 a_smokePosition, SpriteBatch a_spriteBatch)
        {
            foreach (SmokeParticle smokeParticle in m_particleList)
            {
                smokeParticle.Update(a_elapsedTime, a_smokePosition);

                if (smokeParticle.IsActive())
                {
                    float smokeCenter = smokeParticle.GetSize / 2;
                    Vector2 smokePosition = new Vector2(smokeParticle.GetPositionX - smokeCenter, smokeParticle.GetPositionY - smokeCenter);

                    int particleSize = (int)smokeParticle.GetSize;

                    Rectangle particleRectangle = new Rectangle((int)smokePosition.X, (int)smokePosition.Y, particleSize, particleSize);

                    //Fadeing color
                    float a = smokeParticle.GetVisibility();
                    Color particleColor = new Color(a, a, a, a);

                    float rotation = smokeParticle.GetSize / 160;
                    Vector2 origin = new Vector2(m_smokeTexture.Width / 2, m_smokeTexture.Height / 2);

                    a_spriteBatch.Draw(m_smokeTexture, particleRectangle, null, particleColor, rotation, origin, SpriteEffects.None, 0.0f);
                }
            }
        }
    }
}
