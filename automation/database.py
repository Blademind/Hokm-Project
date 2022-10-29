import sqlite3 as lite


class Database:

    def __init__(self, db_path="test.db"):
        """
        setting up the interface that communicates with the database
        """
        self.__con = lite.connect(db_path)

    def close(self):
        """
        closing the connection to the database
        :return: None
        """

        self.__con.close()
        

    def query(self, sql):
        """
        function that execute sql query on the database
        :param sql: str - the sql query
        :return: list - list of the rows of the database
        """

        rows = []

        try:

            cur = self.__con.cursor()
            cur.execute(sql)
            self.__con.commit()
            rows = cur.fetchall()

        except lite.Error as e:
            print(f"sql error: {e}")

        return rows

    def create_table_if_not_exists(self, name, params):
        """
        creates table in the database if the table doesn't exists
        :param name: str - table name
        :param params: tuple (or any iterable) - table params
        :return: None
        """
        table_params = ",".join((param for param in params))
        query = f"CREATE TABLE IF NOT EXISTS {name} ({table_params})"

        self.query(query)

    def check_if_exists(self, name, params):
        """
        checking if data exists in the database
        :param name: str - table name
        :param params - data to check if exists
        :return: bool - exists or not
        """

        search_params = " AND ".join((param[0] + "=" + param[1] for param in params))
        query = f"SELECT * FROM {name} WHERE {search_params}"
        rows = self.query(query)

        return len(rows) != 0
    
    def create_scores_table(self):
        """
        create the table that holds the scores of the players
        :return: None
        """
        self.create_table_if_not_exists("scores", (
            "username TEXT", "score INTEGER"))

    def add_user_if_not_exists(self, username):
        """
        checking if user exists in the database and adding it if not
        :param name: str - table name
        :param params - data to check if exists
        :return: None
        """

        if not self.check_if_exists("scores", (("username", f"'{username}'"), )):
            self.query(f"INSERT INTO scores VALUES('{username}', 0)")

    def increment_score(self, usernames):
        """
        checking if user exists in the database and adding it if not
        :param name: str - table name
        :param params - data to check if exists
        :return: None
        """
        search_params = " OR ".join((f"username='{username}'" for username in usernames))
        query = f"UPDATE scores SET score=score+1 WHERE {search_params}"
        self.query(query)

    def get_scores(self):
        """
        getting the scores from the database and returning a list of them
        :return: list - list[tuple[username, score]]
        """

        query = f"SELECT * FROM scores"
        rows = self.query(query)

        return rows

    