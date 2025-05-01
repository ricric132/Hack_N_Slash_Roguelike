using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalScript {
    public static int score = 0;

    public static void AddScore(int amount) {
        score += amount;
    }
}