using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public static class SpritesLoader
    {
        private static string LoadLink = "https://picsum.photos/200/300";
        
        public static IEnumerator<(Sprite, Action)> LoadSprite()
        {
            using (var request = UnityWebRequest.Get(LoadLink))
            using (var textureHandler = new DownloadHandlerTexture())
            {
                request.downloadHandler = textureHandler;
                var async = request.SendWebRequest();

                while (!async.isDone)
                    yield return (null, null);

                if (request.result.IsOneOf(
                    UnityWebRequest.Result.ConnectionError,
                    UnityWebRequest.Result.ProtocolError,
                    UnityWebRequest.Result.DataProcessingError))
                {
                    yield break;
                }

                var sprite = CreateSprite(textureHandler.texture);
                
                yield return (sprite, () =>
                {
                    UnityEngine.Object.Destroy(sprite.texture);
                    UnityEngine.Object.Destroy(sprite);
                });
            }
        }

        private static Sprite CreateSprite(Texture2D texture)
        {
            var rect = new Rect(0, 0, texture.width, texture.height);
            var pivot = new Vector2(.5f, .5f);
            return Sprite.Create(texture, rect, pivot);
        }
    }
}