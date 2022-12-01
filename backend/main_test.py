import json
import websockets
import asyncio
import random
from game import Game

game=Game()

async def send(ws, type: str, data: str):
    print('>', type, data)
    await ws.send(f'{type}@{data}')


async def nethandle(websocket, path):
    try:
        async for message in websocket:
            idx = message.find('@')
            type = message[0:idx]
            data = message[idx + 1:]
            print('@', type)
            print('<', data)
            await handle(websocket, type, data)
    except:
        print('connection closed')


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


async def handle(ws, type, data):
    global game
    game.clearbuf()
    d=json.loads(data)
    if type == 'NetStartGame':
        game=Game()
        game.restart(d['SaveName'])
        # await send(ws, 'ServerSetMap', json.dumps(genmap(20, 10)))
    if type == 'NetUpgrade':
        pass
        # data_json = json.loads(data)
        # unit = game.handle_upgrade(data_json['ID'])
        # await send(ws, 'ServerSetUnits', json.dumps([unit]))
    for type,content in game.getbuf():
        await send(ws,type,content)


if __name__ == '__main__':
    ip = 'localhost'
    port = 7777
    loop = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)
    print(f'start server at {ip} {port}')
    loop.run_until_complete(
        websockets.serve(nethandle, ip, port))
    loop.run_forever()