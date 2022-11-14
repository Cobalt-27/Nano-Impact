import websockets
import json
import asyncio
import websockets


async def send(ws, type: str, data: str):
    print('>', type, data)
    await ws.send(f'{type}@{data}')


async def nethandle(websocket, path):
    print('echo')
    async for message in websocket:
        idx = message.find('@')
        type = message[0:idx]
        data = message[idx+1:]
        print('@',type)
        print('<',data)
        await handle(websocket, type, data)


async def handle(ws, type, data):
    testdata = {
        'attribute0': 20,
    }
    await send(ws, 'test', json.dumps(testdata))

if __name__ == '__main__':
    ip = 'localhost'
    port = 7777
    loop = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)
    print(f'start server at {ip} {port}')
    loop.run_until_complete(
        websockets.serve(nethandle, ip, port))
    loop.run_forever()
