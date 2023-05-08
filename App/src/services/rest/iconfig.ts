import { IUserResponse } from "./interface";

/**
 * Configuration class needed in base class.
 * The config is provided to the API client at initialization time.
 * API clients inherit from #AuthorizedApiBase and provide the config.
 */
export class IConfig {
  /**
   * Returns a valid value for the Authorization header.
   * Used to dynamically inject the current auth header.
   */


  private token: string = "";

  setToken(token: string) {
    this.token = token;
  }

  getAuthorization(): string {
    return "Bearer " + this.token;
  }
}

export class AuthorizedApiBase {

  authToken = '';
  private readonly config: IConfig;


  protected constructor(config: IConfig) {
    this.config = config;
  }

  setAuthToken(token: string) {
    this.authToken = token;
  }

  //https://github.com/RicoSuter/NSwag/wiki/TypeScriptClientGenerator#inject-an-authorization-header
  protected transformOptions(options: any): Promise<any> {
    /*
    console.log(this.authToken);
    options.headers = options.headers.append('authorization', `Bearer ${this.authToken}`);
    return Promise.resolve(options);*/

    options.headers = {
      ...options.headers,
      Authorization: this.config.getAuthorization(),
    };
    return Promise.resolve(options);
  }
}


