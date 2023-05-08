import { Database, Storage } from "@ionic/storage"

export class AppStorage {
  store: Storage

  private db: Database

  constructor() {
    this.store = new Storage()

    if (this.db === undefined) {
      this.init()
    }
  }

  private init() {
    this.store.create()
  }

  public async get(key: string) {
    this.init()
    return await this.store.get(key)
  }

  public async set(key: string, data: any) {
    this.init()
    return await this.store.set(key, data)
  }

  public async remove(key: string) {
    this.init()
    return await this.store.remove(key)
  }
}
