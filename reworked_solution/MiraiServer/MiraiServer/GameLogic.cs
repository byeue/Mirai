﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MiraiServer
{
    class GameLogic
    {

        public static void Update()
        {
            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    _client.player.Update();
                }
            }
            ThreadManager.UpdateMain();
        }
    }
}
