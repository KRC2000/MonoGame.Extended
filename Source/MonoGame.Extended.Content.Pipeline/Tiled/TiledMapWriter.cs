﻿using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentTypeWriter]
    public class TiledMapWriter : ContentTypeWriter<TiledMapProcessorResult>
    {
        protected override void Write(ContentWriter writer, TiledMapProcessorResult value)
        {
            var map = value.Map;
            //writer.Write(new Color(map.BackgroundColor.R, map.BackgroundColor.G, map.BackgroundColor.B));
            writer.Write(ConvertRenderOrder(map.RenderOrder).ToString());
            writer.Write(map.Width);
            writer.Write(map.Height);
            writer.Write(map.TileWidth);
            writer.Write(map.TileHeight);
            WriteCustomProperties(writer, map.Properties);

            writer.Write(map.Tilesets.Count);

            foreach (var tileset in map.Tilesets)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                writer.Write(Path.GetFileNameWithoutExtension(tileset.Image.Source));
                writer.Write(tileset.FirstGid);
                writer.Write(tileset.TileWidth);
                writer.Write(tileset.TileHeight);
                writer.Write(tileset.Spacing);
                writer.Write(tileset.Margin);
                WriteCustomProperties(writer, tileset.Properties);
            }

            writer.Write(map.Layers.Count);

            foreach (var layer in map.Layers)
            {
                writer.Write(layer.Data.Tiles.Count);

                foreach (var tile in layer.Data.Tiles)
                    writer.Write(tile.Gid);

                writer.Write(layer.Name);
                writer.Write(map.Width);   // layers should have separate width and height
                writer.Write(map.Height);
                WriteCustomProperties(writer, layer.Properties);
            }

            // images layers don't work with TiledSharp apparently.
            //writer.Write(map.ImageLayers.Count);

            //foreach (var imageLayer in map.ImageLayers)
            //{
            //    var assetName = Path.GetFileNameWithoutExtension(imageLayer.Image.Source);
            //    writer.Write(assetName);
            //    writer.Write(imageLayer.Name);
            //    writer.Write(imageLayer.Width);
            //    writer.Write(imageLayer.Height);
            //}
        }

        private static void WriteCustomProperties(ContentWriter writer, List<TmxProperty> properties)
        {
            writer.Write(properties.Count);

            foreach (var mapProperty in properties)
            {
                writer.Write(mapProperty.Name);
                writer.Write(mapProperty.Value);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(TiledMap).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(TiledMapReader).AssemblyQualifiedName;
        }

        private static TiledMapRenderOrder ConvertRenderOrder(TmxRenderOrder renderOrder)
        {
            switch (renderOrder)
            {
                case TmxRenderOrder.RightDown:
                    return TiledMapRenderOrder.RightDown;
                case TmxRenderOrder.RightUp:
                    return TiledMapRenderOrder.RightUp;
                case TmxRenderOrder.LeftDown:
                    return TiledMapRenderOrder.LeftDown;
                case TmxRenderOrder.LeftUp:
                    return TiledMapRenderOrder.LeftUp;
                default:
                    return TiledMapRenderOrder.RightDown;
            }
        }
    }
}