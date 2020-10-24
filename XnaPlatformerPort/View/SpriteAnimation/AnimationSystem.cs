using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace View
{
    ///
    /// Main-class for handling all game sprite animations
    ///
    class AnimationSystem
    {
        //Variables
        private Animator[] m_spriteTextures;
        private SpriteBatch m_spriteBatch;
        private const float m_rotation = 0;
        private const float m_depth = 0.5f;
        private const int m_framesX = 3;
        private const int m_framesY = 4;
        private Stopwatch m_animationTimer;

        //Texture enum
        internal enum Texture
        {
            AVATAR = 0,
            CRAWLER = 1,
            JUMPER = 2,
            SHOOTER = 3,
            COIN = 4,
            SHOOT = 5,
            POWER_UP_ONE = 6,
            EXTRA_LIFE = 7,
            TRAMPOLINE = 8,
            TRIGGER_ONE = 9,
            TRIGGER_TWO = 10
        };

        //Constructor
        public AnimationSystem(SpriteBatch a_spriteBatch)  
        {
            this.m_spriteBatch = a_spriteBatch;
            this.m_animationTimer = new Stopwatch();
            //@PARAM: (Texturname, Origin, Rotation, Scale, Depth, FramesX, FramesY, FPS)
            this.m_spriteTextures = new Animator[11] {new Animator("avatar3", Vector2.Zero, m_rotation, 2f, m_depth, m_framesX, 6, 6),
                                                        new Animator("crawler", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, 4, 5),
                                                        new Animator("jumper", Vector2.Zero, m_rotation, 1.7f, m_depth, m_framesX, 4, 5),
                                                        new Animator("shooter", Vector2.Zero, m_rotation, 1.7f, m_depth, m_framesX, 4, 6),
                                                        new Animator("coin", Vector2.Zero, m_rotation, 1f, m_depth, m_framesX, 4, 5),
                                                        new Animator("shot", Vector2.Zero, m_rotation, 0.7f, m_depth, m_framesX, 4, 5),
                                                        new Animator("jetpack", Vector2.Zero, m_rotation, 1.7f, m_depth, m_framesX, 1, 4),
                                                        new Animator("extra-life", Vector2.Zero, m_rotation, 1.2f, m_depth, m_framesX, 1, 4),
                                                        new Animator("trampoline", Vector2.Zero, m_rotation, 1.5f, m_depth, m_framesX, 1, 4),
                                                        new Animator("triggerOne", Vector2.Zero, m_rotation, 1.7f, m_depth, m_framesX, 1, 4),
                                                        new Animator("triggerTwo", Vector2.Zero, m_rotation, 1.7f, m_depth, m_framesX, 1, 4)};
        }

        //Loading all textures
        internal void LoadContent(Microsoft.Xna.Framework.Content.ContentManager a_content)
        {
            foreach (Animator sprite in m_spriteTextures)
            {
                sprite.Load(a_content);
            }
        }

        //Updating and drawing frames via UpdateFrame & DrawFrame
        internal void UpdateAndDraw(float a_elapsedTime, Color a_color, Vector2 a_texturePos, Model.State.ID a_animation, Texture a_texture)
        {
            int frameY = -1;
            int frameX = -1;
            bool staticAnimation = false;
            bool verticalAnimation = false;

            switch (a_animation)
            {
                case Model.State.ID.CLIMBING_DOWN:
                    frameY = 0;
                    break;
                case Model.State.ID.CLIMBING:
                    frameY = 3;
                    break;
                case Model.State.ID.MOVING_RIGHT:
                    frameY = 2;
                    break;
                case Model.State.ID.MOVING_LEFT:
                    frameY = 1;
                    break;
                case Model.State.ID.FACING_CAMERA:
                    frameX = 1;
                    frameY = 0;
                    staticAnimation = true;
                    break;
                case Model.State.ID.FACING_LEFT:
                    frameX = 1;
                    frameY = 1;
                    staticAnimation = true;
                    break;
                case Model.State.ID.FACING_RIGHT:
                    frameX = 1;
                    frameY = 2;
                    staticAnimation = true;
                    break;
                case Model.State.ID.FACING_AWAY:
                    frameX = 1;
                    frameY = 3;
                    staticAnimation = true;
                    break;
                case Model.State.ID.JUMPING_RIGHT:
                    frameX = 0;
                    frameY = 2;
                    staticAnimation = true;
                    break;
                case Model.State.ID.JUMPING_LEFT:
                    frameX = 2;
                    frameY = 1;
                    staticAnimation = true;
                    break;
                case Model.State.ID.FLYING_RIGHT:
                    frameY = 5;
                    break;
                case Model.State.ID.FLYING_LEFT:
                    frameY = 4;
                    break;
                case Model.State.ID.IS_DEAD_RIGHT:
                    frameX = 0;
                    frameY = 3;
                    staticAnimation = true;
                    break;
                case Model.State.ID.IS_DEAD_LEFT:
                    frameX = 2;
                    frameY = 3;
                    staticAnimation = true;
                    break;
                case Model.State.ID.VERTICAL_ANIMATION:
                    frameX = 1;
                    verticalAnimation = true;
                    break;
                case Model.State.ID.STATIC_ONE:
                    frameX = 0;
                    frameY = 0;
                    staticAnimation = true;
                    break;
                case Model.State.ID.STATIC_TWO:
                    frameX = 1;
                    frameY = 0;
                    staticAnimation = true;
                    break;
            }

            if (staticAnimation)
            {
                m_spriteTextures[Convert.ToInt32(a_texture)].StaticTexture(a_elapsedTime, frameX, frameY);
                
            }
            else if (verticalAnimation)
            {
                m_spriteTextures[Convert.ToInt32(a_texture)].AnimateVerticalSprite(a_elapsedTime, frameX);
            }
            else
            {
                m_spriteTextures[Convert.ToInt32(a_texture)].AnimateSprite(a_elapsedTime, frameY);
            }

            m_spriteTextures[Convert.ToInt32(a_texture)].DrawFrame(m_spriteBatch, a_texturePos, frameY, a_color);

        }

        //Seperate updating class for multible animations of the same sort
        internal void UpdateMultibleAnimations(float a_elapsedTime, Texture a_texture, int a_frameY)
        {
            m_spriteTextures[Convert.ToInt32(a_texture)].AnimateSprite(a_elapsedTime, a_frameY);
        }
       
        //Seperate drawing class for multible animations of the same sort
        internal void DrawMultibleAnimations(Color a_color, Vector2 a_texturePos, int a_frameY, Texture a_texture)
        {
            m_spriteTextures[Convert.ToInt32(a_texture)].DrawFrame(m_spriteBatch, a_texturePos, a_frameY, a_color);
        }

    }
}
