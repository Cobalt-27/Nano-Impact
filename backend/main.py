import json
import os.path

import websockets
import asyncio
import random
from game import Game

from websockets.server import WebSocketServerProtocol

game = Game()
clients = {}


async def send(ws, type: str, data: str):
    print('@', type)
    print('>', data)
    await ws.send(f'{type}@{data}')


async def nethandle(websocket: WebSocketServerProtocol, path):
    # try:
    global clients
    addr = websocket.remote_address
    clients[addr[0]] = websocket
    async for message in websocket:
        idx = message.find('@')
        type = message[0:idx]
        data = message[idx + 1:]
        print('@', type)
        print('<', data)
        await handle(websocket, type, data)
    # except Exception as e:
    #     print(repr(e))
    # print('connection closed')


async def handle(ws: WebSocketServerProtocol, type, data):
    global game
    game.clearbuf()
    d = json.loads(data)
    if type == 'NetGreet':
        game.getbuf().append(('NetSetSaveInfo', get_save(), -1, 0))
    elif type == 'NetStartGame':
        game = Game()
        game.restart(d['SaveName'])
        # await send(ws, 'ServerSetMap', json.dumps(genmap(20, 10)))
    elif type == 'NetUpgrade':
        pass
        # data_json = json.loads(data)
        # unit = game.handle_upgrade(data_json['ID'])
        # await send(ws, 'ServerSetUnits', json.dumps([unit]))
    elif type == 'NetEndRound':
        game.handle_endRound()
    elif type == 'NetInteract':
        game.handle_interact(d['From'], d['To'])
    elif type == 'NetMove':
        game.handle_move(d['ID'], d['Row'], d['Col'])
    elif type == 'NetAssignRelic':
        game.handle_assignRelic(d['ID'], d['Relic'])
    elif type == 'NetAddBuilding':
        game.handle_addBuilding(d['Type'], d['Row'], d['Col'])
    elif type == 'NetSave':
        game.handle_save(d['Name'])
    elif type == 'NetQuit':
        game.handle_quit()
    elif type == 'NetRollback':
        game.handle_rollBack()

    for type, content, target, delay in game.getbuf():
        for ws in clients:
            await send(clients[ws], type, content)
            await asyncio.sleep(delay)


def get_save():
    if not os.path.exists('./Saving'):
        os.makedirs('./Saving')
    return json.dumps(os.listdir('./Saving'))


if __name__ == '__main__':
    if not os.path.exists('./Rollback'):
        os.makedirs('./Rollback')
    get_save()
    ip = '0.0.0.0'
    port = 7777
    loop = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)
    print(f'start server at {ip} {port}')
    loop.run_until_complete(
        websockets.serve(nethandle, ip, port))
    loop.run_forever()
