# Qala CLI Documentation

## `login`

**Description:** this commands initiates the process for the user to 
login.

### Examples:
```sh
login
```

## `config`

**Description:** this command creates enables the configuration of the Qala CLI 
(for automation purposes).

### Options:
| Option | Description |
|--------|-------------|
| `e` / `env` | The environment ID to use for the request. |
| `k` / `key` | The API key to use for the request. |

### Examples:
```sh
config -k <API_KEY> -e <ENV_ID>
```

## `environment`

**Description:** No description available.

### `create`

**Description:** this command creates a new environment for the Qala 
CLI.

### Options:
| Option | Description |
|--------|-------------|
| `disableSchemaValidation` | Disable schema validation. |
| `n` / `name` | The name of the environment. |
| `r` / `region` | The region of the environment. |
| `t` / `type` | The type of the environment. |

### Examples:
```sh
environment create -n <NAME> -r  <REGION> -t <TYPE> --disableSchemaValidation
```

### `current`

**Description:** this command retrieves the current environment for the Qala 
CLI.

### Examples:
```sh
environment current
```

### `set`

**Description:** this command sets the current environment for the Qala 
CLI.

### Options:
| Option | Description |
|--------|-------------|
| `e` / `environmentId` | The id of the environment. |

### Examples:
```sh
environment set -e <ENVIRONMENT_ID>
```

### `update`

**Description:** this command updates the current environment for the Qala 
CLI.

### Options:
| Option | Description |
|--------|-------------|
| `disableSchemaValidation` | Disable schema validation. |

### Examples:
```sh
environment update --disableSchemaValidation
```

## `events`

**Description:** No description available.

### `list`

**Description:** this command lists all the event types available in your 
environment.

### Examples:
```sh
events ls
```

### `inspect`

**Description:** this command retrieves the event type with the specified 
ID.

### Options:
| Option | Description |
|--------|-------------|

### Examples:
```sh
events inspect <EVENT_TYPE_ID>
```

## `topics`

**Description:** No description available.

### `list`

**Description:** this command lists all the topics available in your 
environment.

### Examples:
```sh
topics ls
```

### `name`

**Description:** this command retrieves the topic with the specified 
NAME.

### Options:
| Option | Description |
|--------|-------------|

### Examples:
```sh
topics name <NAME>
```

### `create`

**Description:** this command creates a new topic for the Qala 
CLI.

### Options:
| Option | Description |
|--------|-------------|
| `d` / `description` | The description of the topic. |
| `e` / `event-type-ids` | The comma separated list of event type ids. |
| `n` / `name` | The name of the topic. |

### Examples:
```sh
topics create -n <NAME> -d  <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_IDS>
```

### `update`

**Description:** this command updates an existing topic for the Qala 
CLI.

### Options:
| Option | Description |
|--------|-------------|
| `d` / `description` | The description of the topic. |
| `e` / `event-type-ids` | The comma separated list of event type ids. |

### Examples:
```sh
topics update <NAME> -d <DESCRIPTION>  -e <EVENTS_COMMA_SEPERATED_IDS>
```

## `sources`

**Description:** No description available.

### `list`

**Description:** this command lists all the sources available in your 
environment.

### Examples:
```sh
qala sources ls
```

### `name`

**Description:** this command retrieves the source with the specified 
NAME.

### Options:
| Option | Description |
|--------|-------------|

### Examples:
```sh
qala sources name <NAME>
```

### `create`

**Description:** this command creates a new source for the Qala 
CLI.

### Options:
| Option | Description |
|--------|-------------|
| `algorithm` | The algorithm of the source when authentication type is 
jwt (RSA or HSA). |
| `apiKeyName` | The name of the api key when authentication type is api 
key. |
| `apiKeyValue` | The value of the api key when authentication type is api 
key. |
| `audience` | The audience of the source when authentication type is 
jwt. |
| `a` / `authenticationType` | The authentication type of the source (Basic, ApiKey or 
JWT). |
| `d` / `description` | The description of the source. |
| `i` / `ip-whitelisting` | The comma separated list of the ips allowed to access the
source. |
| `issuer` | The issuer of the source when authentication type is 
jwt. |
| `m` / `methods` | The comma separated list of the http methods available to
the source. |
| `n` / `name` | The Name of the source. |
| `password` | The password of the source when authentication type is 
basic. |
| `publicKey` | The public key of the source when authentication type is 
jwt and algorithm is RSA. |
| `secret` | The secret of the source when authentication type is jwt 
and algorithm is HSA. |
| `username` | The username of the source when authentication type is 
basic. |

### Examples:
```sh
qala sources create -n <NAME> -d  <DESCRIPTION> -m <METHODS_COMMA_SEPERATED> -i  <IP_WHITELISTING_COMMA_SEPERATED> -a <AUTHENTICATION_TYPE>
```

### `update`

**Description:** this command updates an existing source for the Qala 
CLI.

### Options:
| Option | Description |
|--------|-------------|
| `algorithm` | The algorithm of the source when authentication type is 
jwt (RSA or HSA). |
| `apiKeyName` | The name of the api key when authentication type is api 
key. |
| `apiKeyValue` | The value of the api key when authentication type is api 
key. |
| `audience` | The audience of the source when authentication type is 
jwt. |
| `a` / `authenticationType` | The authentication type of the source (Basic, ApiKey or 
JWT). |
| `d` / `description` | The description of the source. |
| `i` / `ip-whitelisting` | The comma separated list of the ips allowed to access the
source. |
| `issuer` | The issuer of the source when authentication type is 
jwt. |
| `m` / `methods` | The comma separated list of the http methods available to
the source. |
| `n` / `name` | The new name of the source. |
| `password` | The password of the source when authentication type is 
basic. |
| `publicKey` | The public key of the source when authentication type is 
jwt and algorithm is RSA. |
| `secret` | The secret of the source when authentication type is jwt 
and algorithm is HSA. |
| `username` | The username of the source when authentication type is 
basic. |

### Examples:
```sh
qala sources update <NAME> -d  <DESCRIPTION> -m <METHODS_COMMA_SEPERATED> -i  <IP_WHITELISTING_COMMA_SEPERATED> -a <AUTHENTICATION_TYPE>
```

### `delete`

**Description:** this command deletes the source with the specified 
NAME.

### Options:
| Option | Description |
|--------|-------------|

### Examples:
```sh
qala sources delete -n <NAME>
```

## `subscriptions`

**Description:** No description available.

### `list`

**Description:** this command lists all the subscriptions available in your 
environment.

### Options:
| Option | Description |
|--------|-------------|
| `t` / `topic` | No description. |

### Examples:
```sh
qala subscriptions ls -t <TOPIC_NAME>
```

### `inspect`

**Description:** this command retrieves the subscription with the specified 
ID.

### Options:
| Option | Description |
|--------|-------------|
| `s` / `subscription` | No description. |
| `t` / `topic` | No description. |

### Examples:
```sh
qala subscriptions i -t <TOPIC_NAME> -s  <SUBSCRIPTION_ID>
```

### `create`

**Description:** this command creates a new subscription for the Qala 
CLI.

### Options:
| Option | Description |
|--------|-------------|
| `d` / `description` | The description of the subscription. |
| `e` / `event-type-ids` | The comma separated list of event type ids. |
| `m` / `max-attempts` | The maximum delivery attempts of the 
subscription. |
| `n` / `name` | The name of the subscription. |
| `t` / `topic` | The name of the topic. |
| `u` / `url` | The webhook url of the subscription. |

### Examples:
```sh
qala sub create -n <SUBSCRIPTION_NAME> -t  <TOPIC_NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_IDS>  -u <WEBHOOK_URL> -m <MAX_DELIVERY_ATTEMPTS>
```

### `update`

**Description:** this command updates an existing subscription for the Qala 
CLI.

### Options:
| Option | Description |
|--------|-------------|
| `d` / `description` | The description of the subscription. |
| `e` / `events` | The comma separated list of event type ids. |
| `m` / `max-attempts` | The maximum delivery attempts of the 
subscription. |
| `n` / `name` | The name of the subscription. |
| `t` / `topic` | The name of the topic. |
| `u` / `url` | The webhook url of the subscription. |

### Examples:
```sh
qala sub update <SUBSCRIPTION_ID> -t  <TOPIC_NAME> -d <DESCRIPTION> -e <EVENTS_COMMA_SEPERATED_IDS>  -u <WEBHOOK_URL> -m <MAX_DELIVERY_ATTEMPTS>
```

### `delete`

**Description:** this command deletes the subscription with the specified 
ID.

### Options:
| Option | Description |
|--------|-------------|
| `s` / `subscription` | The id of the subscription. |
| `t` / `topic` | The name of the topic. |

### Examples:
```sh
qala sub delete -t <TOPIC_NAME> -s  <SUBSCRIPTION_ID>
```

### `secret`

**Description:** No description available.

#### `inspect`

**Description:** this command retrieves the webhook secret for the 
subscription with the specified ID.

### Options:
| Option | Description |
|--------|-------------|
| `s` / `subscription-id` | The id of the subscription. |
| `t` / `topic` | The name of the topic. |

### Examples:
```sh
qala sub secret i -t <TOPIC_NAME> -s  <SUBSCRIPTION_ID>
```

#### `rotate`

**Description:** this command rotates the webhook secret for the 
subscription with the specified ID.

### Options:
| Option | Description |
|--------|-------------|
| `s` / `subscription-id` | The id of the subscription. |
| `t` / `topic` | The name of the topic. |

### Examples:
```sh
qala sub secret rotate -t <TOPIC_NAME> -s  <SUBSCRIPTION_ID>
```

