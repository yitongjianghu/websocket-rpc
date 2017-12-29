﻿#region License
// Copyright © 2018 Darko Jurić
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;

namespace WebSocketRPC
{
    class LocalBinder<TObj> : Binder, ILocalBinder<TObj>
    {
        LocalInvoker<TObj> lInvoker = null;

        public LocalBinder(Connection connection, TObj obj)
            : base(connection)
        {
            lInvoker = new LocalInvoker<TObj>();
            Object = obj;

            Connection.OnReceive += async d =>
            {
                var msg = Request.FromJson(d);
                if (msg.IsEmpty) return;

                var result = await lInvoker.InvokeAsync(Object, msg);
                await Connection.SendAsync(result.ToJson());
            };

            Connection.OnClose += () =>
            {
                (Object as IDisposable)?.Dispose();
            };
        }

        public TObj Object { get; private set; }
    }
}