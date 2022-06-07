using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Constants
{
    public static int PALM_CENTER_MARKER_ID = 61;
    public static int NUM_TRAJECTORY_POINTS = 100;
}

public static class TextConstants
{
    public static string START_GAME       = "\n\nHello!\n\nWelcome to HomeCourt!";
    public static string AWAITING_SWING   = "\nMatch the guide as closely as possible to learn how to do a proper forehand.\n\n" +
                                            "Swing through the red spheres to get your swing evaluated!";
    public static string SWING_FEEDBACK_A = "\nAmazing Swing!\nKeep it up!";
    public static string SWING_FEEDBACK_B = "\nGreat One!\nNow swing for perfection!";
    public static string SWING_FEEDBACK_C = "\nGood Swing!\nTry to follow the guide even closer for better results!";
    public static string SWING_FEEDBACK_D = "\nNot bad, but could be better!\nFollow the guide as close as possible!";
    public static string SWING_FEEDBACK_F = "\nYikes! Try that again!";
}

public static class ScoreConstants
{
    public static float SCORE_THRESH_A = 0.75f;
    public static float SCORE_THRESH_B = 0.82f;
    public static float SCORE_THRESH_C = 0.90f;
    public static float SCORE_THRESH_D = 0.98f;
}

public static class Utilities
{
    public static List<Vector3> Discretize(List<Vector3> trajectory)
    {
        List<Vector3> discretizedTrajectory = new List<Vector3>(Constants.NUM_TRAJECTORY_POINTS);
        List<Vector3> points;
        float t = 0.0f;

        for (int i = 0; i < Constants.NUM_TRAJECTORY_POINTS; i++)
        {
            t = Mathf.InverseLerp(0, Constants.NUM_TRAJECTORY_POINTS, i);
            
            points = new List<Vector3>(trajectory);

            for (int j = trajectory.Count - 1; j > 0; j--)
            {
                for (int k = 0; k < j; k++)
                {
                    points[k] = (1-t)*points[k] + t*points[k+1];
                }
            }

            discretizedTrajectory.Add(points[0]);
        }

        return discretizedTrajectory;
    }
}

[Serializable]
public struct State 
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
}

[Serializable]
public struct Snapshot
{
    public float Time;
    public State[] States;
}