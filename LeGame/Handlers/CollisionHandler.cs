﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
 
using Microsoft.Xna.Framework.Input;
using LeGame.Models.Characters;
using LeGame.Interfaces;
using LeGame.Models;
using Microsoft.Xna.Framework.Graphics;


namespace LeGame.Handlers
{
    internal static class CollisionHandler
    {
        public static Object Collide(Character character)
        {
            List<Rectangle> objectRects = new List<Rectangle>();
            List<GameObject> daaa = character.Level.Assets.Concat(character.Level.Enemies).ToList(); // certainly NEEDS to be revamped
            foreach (var asset in daaa)// character.Level.Assets)
            {
                Rectangle obj = new Rectangle(
                    (int)asset.Position.X,
                    (int)asset.Position.Y,
                    GfxHandler.GetWidth(asset),
                    GfxHandler.GetHeight(asset));
                
                if ((asset is ICollisionable || asset is IPickable) 
                    && GfxHandler.GetBBox(character).Intersects(obj))
                {
                    return asset;
                }
                else
                {
                    continue;
                }
            }
            return 1;
        }
        public static void Reaction(Character character, Keys key)
        {
            if (CollisionHandler.Collide(character) != (object)1)
            {
                 Vector2 temp = new Vector2(character.Position.X, character.Position.Y);
               
                //movement reactions
            
                if (CollisionHandler.Collide(character) is ICollisionable && key == Keys.D)
                {
                    temp.X -= character.Speed;
                    character.Position = temp;
                }
                if (CollisionHandler.Collide(character) is ICollisionable && key == Keys.W)
                {
                    temp.Y += character.Speed;
                    character.Position = temp;
                }
                if (CollisionHandler.Collide(character) is ICollisionable && key == Keys.S)
                {
                    temp.Y -= character.Speed;
                    character.Position = temp;
                }
                if (CollisionHandler.Collide(character) is ICollisionable && key == Keys.A)
                {
                    temp.X += character.Speed;
                    character.Position = temp;
                }
            
                if (CollisionHandler.Collide(character) is IPickable)
                {
                    GameObject item = (GameObject)CollisionHandler.Collide(character);
                    Console.Beep(8000,50); // legit cool gold-pickup sound
                    character.Level.Assets.Remove(item);
                }
            }

        }
    }
}