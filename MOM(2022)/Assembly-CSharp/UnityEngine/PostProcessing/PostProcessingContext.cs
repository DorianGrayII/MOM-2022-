﻿namespace UnityEngine.PostProcessing
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class PostProcessingContext
    {
        public PostProcessingProfile profile;
        public Camera camera;
        public MaterialFactory materialFactory;
        public RenderTextureFactory renderTextureFactory;

        public void Interrupt()
        {
            this.interrupted = true;
        }

        public PostProcessingContext Reset()
        {
            this.profile = null;
            this.camera = null;
            this.materialFactory = null;
            this.renderTextureFactory = null;
            this.interrupted = false;
            return this;
        }

        public bool interrupted { get; private set; }

        public bool isGBufferAvailable
        {
            get
            {
                return (this.camera.actualRenderingPath == RenderingPath.DeferredShading);
            }
        }

        public bool isHdr
        {
            get
            {
                return this.camera.allowHDR;
            }
        }

        public int width
        {
            get
            {
                return this.camera.pixelWidth;
            }
        }

        public int height
        {
            get
            {
                return this.camera.pixelHeight;
            }
        }

        public Rect viewport
        {
            get
            {
                return this.camera.rect;
            }
        }
    }
}

