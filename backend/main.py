import json
import os.path

import websockets
import asyncio
import random
from game import Game

from websockets.server import WebSocketServerProtocol

game = Game()
clients = []
multiplayer = False


class ClientInfo:
    def __init__(self, addr, name, index, ws):
        self.addr = addr
        self.name = name
        self.index = index
        self.ws = ws


def search_client(x):
    for client in clients:
        if client.addr == x or client.name == x or client.index == x:
            return client
    return None


async def send(ws, type: str, data: str):
    print('@', type)
    print('>', data)
    await ws.send(f'{type}@{data}')


async def nethandle(websocket: WebSocketServerProtocol, path):
    try:
        async for message in websocket:
            idx = message.find('@')
            type = message[0:idx]
            data = message[idx + 1:]
            print('@', type)
            print('<', data)
            await handle(websocket, type, data)
    except websockets.ConnectionClosedError:
        print(f'Connection closed with {websocket.remote_address}')
        clients.remove(search_client(websocket.remote_address))
    # except Exception as e:
    #     print(repr(e))
    # print('connection closed')


async def handle(ws: WebSocketServerProtocol, type, data):
    global game
    global multiplayer
    global clients
    game.clearbuf()
    d = json.loads(data)
    # handle greet
    addr = ws.remote_address
    if multiplayer:
        if type != 'NetGreet' and len(clients) < 2:
            # await ws.close()
            return
        if type == 'NetGreet' and len(clients) >= 2 and search_client(addr) is None:
            print(f'close socket with {addr} because full client')
            await ws.close()
            return
        if type == 'NetGreet':
            clients.append(ClientInfo(addr, d['ClientName'], len(clients), ws))
            game.getbuf().append(('NetSetSaveInfo', get_save(), -1, 0))
            if len(clients) == 1:
                game.getbuf().append(('ClientShow', generate_message('Waiting'), 0, 0))
        # Throw wrong packets
        c = search_client(addr)
        cur_index = 1
        if game.player:
            cur_index = 0
        print(cur_index)
        print(c.index)
        if c is None or (type not in ['NetGreet', 'NetStartGame'] and c.index != cur_index):
            print(f'Unsupported {type} command from opposite')
            return
    else:
        if type == 'NetGreet':
            client = search_client(d['ClientName'])
            if client is not None:
                await client.ws.close()
                clients.remove(client)
                print(f'close socket with {client.addr} because reconnect')
            if len(clients) == 0 or search_client(addr) is not None:
                clients.append(ClientInfo(addr, d['ClientName'], len(clients), ws))
                game.getbuf().append(('NetSetSaveInfo', get_save(), -1, 0))
            else:
                print(f'close socket with {addr} because full client')
                await ws.close()
                return
    if type == 'NetStartGame':
        game = Game()
        game.restart(d['SaveName'])
        game.enable_ai = False
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
    print('prepare to send')
    for type, content, target, delay in game.getbuf():
        await asyncio.sleep(delay)
        if target == -1:
            for c in clients:
                await send(c.ws, type, content)
        else:
            await send(search_client(target).ws, type, content)

        # for ws in clients:
        #     await send(clients[ws], type, content)


def get_save():
    if not os.path.exists('./Saving'):
        os.makedirs('./Saving')
    saves = []
    for file in os.listdir('./Saving'):
        save = dict()
        save['Name'] = file
        save['Description'] = 'No Description'
        saves.append(save)
    to_dump = dict()
    to_dump['SaveInfoList'] = saves
    return json.dumps(to_dump)


def generate_message(str):
    message = dict()
    message['content'] = str
    return json.dumps(message)


if __name__ == '__main__':
    if not os.path.exists('./RollBack'):
        os.makedirs('./RollBack')
    # print(get_save())
    ip = '0.0.0.0'
    port = 7777
    loop = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)
    print(f'start server at {ip} {port}')
    loop.run_until_complete(
        websockets.serve(nethandle, ip, port))
    loop.run_forever()
