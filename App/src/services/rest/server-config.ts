class ServerConfig {
  //  private _host = "https://localhost:7244";

  //    private _datahost = "https://localhost:7170";
  //
  private _baseurl = "http://localhost:9080";
  private _api = "/mgmt";
  private _data = "";

  public get host(): string {
    return this._baseurl + this._api;
  };

  public get datahost(): string {
    return this._baseurl + this._data;
  };

}

export default new ServerConfig()
