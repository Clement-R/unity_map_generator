using UnityEngine;
using System.Collections;

public class Position {
    public int x;
    public int y;

    public Position(int x, int y) {
        this.x = x;
        this.y = y;
    }

    // Equality operator overloading
    public static bool operator ==(Position a, Position b) {
        if (a.x == b.x && a.y == b.y) {
            return true;
        } else {
            return false;
        }
    }

    // Inequality operator overloading
    public static bool operator !=(Position a, Position b) {
        if (a.x != b.x || a.y != b.y) {
            return true;
        }
        else {
            return false;
        }
    }
}