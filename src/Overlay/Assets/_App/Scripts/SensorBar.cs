﻿using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

namespace Ideum {

  public class SensorBar : MonoBehaviour {

    public SVGImage Background;

    public Color ActiveColor;
    public Color InactiveColor;

    private RectTransform _rect;

    private void Awake() {
      _rect = GetComponent<RectTransform>();

      Background.color = InactiveColor;
    }

    public void Resize(Vector2 Size) {
      _rect.sizeDelta = Size;
    }

    public void SetStatus(bool active) {
      Background.color = active ? ActiveColor : InactiveColor;
    }
  }
}
