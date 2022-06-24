using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unisave;
using Unisave.Facets;
using Unisave.Facades;
using Unisave.Authentication.Middleware;
using Unisave.Utils;

public class SaveFacet : Facet
{
    /// <summary>
    /// Client can call this facet method and receive a greeting
    /// Replace this with your own server-side code
    /// </summary>
    public string GreetClient()
    {
        return "Hello client! I'm the server!";
    }

    public void Register(string user, string password)
    {
        var player = DB.TakeAll<PlayerEntity>()
            .Filter(p => p.name == user)
            .First();

        if (player != null)
        {
            Debug.Log("User Already Exists");
            return;
        }

        var newPlayer = new PlayerEntity
        {
            name = user,
            password = Hash.Make(password)
    };
        Debug.Log("New User Created");

        newPlayer.Save();
        Auth.Login(newPlayer);

        if (newPlayer == null)
            Log.Info("Nobody is logged in.");
        else
            Log.Info(newPlayer.name + " is logged in.");
    }

    public bool UserLogin(string user, string password)
    {
        var player = Auth.GetPlayer<PlayerEntity>();
        if (player == null)
        {
            var userMatched = DB.TakeAll<PlayerEntity>()
                .Filter(p => p.name == user)
                .Get();
            bool matches = Hash.Check(password, userMatched[0].password);
            if (matches)
            {
                Auth.Login(userMatched[0]);
                Log.Info(userMatched[0].name + " is logged in.");
                return true;
            }
            else
            {
                Log.Info("incorrect user/password");
                return false;
            }
            
        }
        else
        {
            Log.Info(player.name + " is logged in.");
            return true;
        }

               
    }

    public static bool loggedIn = false;

    public string CheckLogin()
    {
        var player = Auth.GetPlayer<PlayerEntity>();
        if (player != null)
        {
            Debug.Log("Already logged in, Loading Game");
            return player.name;
        }
        else
        {
            Debug.Log("Not logged in");
            return "";
        }
    }

    public void Logout()
    {
        Auth.Logout();
    }
}
