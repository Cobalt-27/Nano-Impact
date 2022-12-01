import json
import websockets
import asyncio
import random
# from game import Game

to_send = []


def send(type: str, data: str):
    print('>', type, data)
    for ws in to_send:
        try:
            asyncio.new_event_loop().run_until_complete(ws.send(f'{type}@{data}'))
        except Exception:
            to_send.remove(ws)


async def async_send(ws, type: str, data: str):
    print('>', type, data)
    await ws.send(f'{type}@{data}')


async def nethandle(websocket, path):
    print('echo')
    to_send.append(websocket)
    async for message in websocket:
        idx = message.find('@')
        type = message[0:idx]
        data = message[idx + 1:]
        print('@', type)
        print('<', data)
        asyncio.get_event_loop().run_in_executor(None, handle, websocket, type, data)


def genmap(row, col) -> dict:
    m = {}
    m['Row'] = row
    m['Col'] = col
    m['Blocks'] = []
    for i in range(row):
        for j in range(col):
            b = {}
            b['Row'] = i
            b['Col'] = j
            b['Height'] = random.random()
            b['Type'] = random.randint(0, 4)
            m['Blocks'].append(b)
    return m


def handle(ws, type, data):
    pass
    if type == 'NetStartGame':
        # game.restart(20, 20, None, False, None)
        return
    if type == 'NetUpgrade':
        data_json = json.loads(data)
        game.handle_upgrade(data_json['ID'])


async def wait_input():
    while True:
        read = await loop.run_in_executor(executor=None, func=input)
        print(read)
        await loop.run_in_executor(None, send, 'ClientPrint', 'TestMessage')


if __name__ == '__main__':
    ip = 'localhost'
    port = 7777
    game = Game(send)
    loop = asyncio.new_event_loop()
    loop2 = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)
    print(f'start server at {ip} {port}')
    loop.run_until_complete(
        websockets.serve(nethandle, ip, port))
    loop.run_until_complete(wait_input())
    loop.run_forever()
